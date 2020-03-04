using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Goldnote.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Drawing;
using Goldnote.Models;
using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using System.Diagnostics;


using System.Numerics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using SixLabors.Shapes;
namespace Goldnote
{
    public class Options
    {
        /// <summary>
        /// 行き先を定義した集合
        /// </summary>
        public IEnumerable<SelectListItem> Destinations { get; } =new List<SelectListItem> {

            new SelectListItem { Text="ユニー行き",Value="true"},

            new SelectListItem { Text="本部行き",Value="false"}

        };
    }

    public enum SslMode
    {
        Require,
        Disable,
        Prefer
    }

    public class PostgreSqlConnectionStringBuilder : DbConnectionStringBuilder
    {
        private string _database;
        private string _host;
        private string _password;
        private bool _pooling;
        private int _port;
        private string _username;
        private bool _trustServerCertificate;
        private SslMode _sslMode;

        public PostgreSqlConnectionStringBuilder(string uriString)
        {
            ParseUri(uriString);
        }

        public string Database
        {
            get => _database;
            set
            {
                base["database"] = value;
                _database = value;
            }
        }

        public string Host
        {
            get => _host;
            set
            {
                base["host"] = value;
                _host = value;
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                base["password"] = value;
                _password = value;
            }
        }

        public bool Pooling
        {
            get => _pooling;
            set
            {
                base["pooling"] = value;
                _pooling = value;
            }
        }

        public int Port
        {
            get => _port;
            set
            {
                base["port"] = value;
                _port = value;
            }
        }

        public string Username
        {
            get => _username;
            set
            {
                base["username"] = value;
                _username = value;
            }
        }

        public bool TrustServerCertificate
        {
            get => _trustServerCertificate;
            set
            {
                base["trust server certificate"] = value;
                _trustServerCertificate = value;
            }
        }

        public SslMode SslMode
        {
            get => _sslMode;
            set
            {
                base["ssl mode"] = value.ToString();
                _sslMode = value;
            }
        }

        public override object this[string keyword]
        {
            get
            {
                if (keyword == null) throw new ArgumentNullException(nameof(keyword));
                return base[keyword.ToLower()];
            }
            set
            {
                if (keyword == null) throw new ArgumentNullException(nameof(keyword));

                switch (keyword.ToLower())
                {
                    case "host":
                        Host = (string)value;
                        break;

                    case "port":
                        Port = Convert.ToInt32(value);
                        break;

                    case "database":
                        Database = (string)value;
                        break;

                    case "username":
                        Username = (string)value;
                        break;

                    case "password":
                        Password = (string)value;
                        break;

                    case "pooling":
                        Pooling = Convert.ToBoolean(value);
                        break;

                    case "trust server certificate":
                        TrustServerCertificate = Convert.ToBoolean(value);
                        break;

                    case "sslmode":
                        SslMode = (SslMode)value;
                        break;

                    default:
                        throw new ArgumentException(string.Format("Invalid keyword '{0}'.", keyword));
                }
            }
        }

        public override bool ContainsKey(string keyword)
        {
            return base.ContainsKey(keyword.ToLower());
        }

        private void ParseUri(string uriString)
        {
            var isUri = Uri.TryCreate(uriString, UriKind.Absolute, out var uri);

            if (!isUri) throw new FormatException(string.Format("'{0}' is not a valid URI.", uriString));

            Host = uri.Host;
            Port = uri.Port;
            Database = uri.LocalPath.Substring(1);
            Username = uri.UserInfo.Split(':')[0];
            Password = uri.UserInfo.Split(':')[1];
        }
    }
    public class FileChecker
    {
        private static byte[] getBytes(IFormFile file)
        {
            byte[] fileBytes;
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                fileBytes = ms.ToArray();
            }

            return fileBytes;
        }






        private static Dictionary<string, List<byte[]>> fileSignature = new Dictionary<string, List<byte[]>>
                    {
                    { ".PNG", new List<byte[]> { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } } },
                    { ".JPG", new List<byte[]>
                                    {
                                              new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                                              new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
                                              new byte[] { 0xFF, 0xD8, 0xFF, 0xE8 }
                                    }
                                    },
                    { ".JPEG", new List<byte[]>
                                        {
                                            new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                                            new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                                            new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 }
                                        }
                                        },
                    { ".GIF", new List<byte[]> { new byte[] { 0x47, 0x49, 0x46, 0x38 } } }
                };

        public static bool IsValidFile(IFormFile file)
        {
            var data = getBytes(file);
            return IsValidFileExtension(file.FileName, data, null);

        }
        public static bool IsValidFileExtension(string fileName, byte[] fileData, byte[] allowedChars)
        {
            if (string.IsNullOrEmpty(fileName) || fileData == null || fileData.Length == 0)
            {
                return false;
            }

            bool flag = false;
            string ext = System.IO.Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(ext))
            {
                return false;
            }

            ext = ext.ToUpperInvariant();

            if (ext.Equals(".TXT") || ext.Equals(".CSV") || ext.Equals(".PRN"))
            {
                foreach (byte b in fileData)
                {
                    if (b > 0x7F)
                    {
                        if (allowedChars != null)
                        {
                            if (!allowedChars.Contains(b))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            if (!fileSignature.ContainsKey(ext))
            {
                return false;
            }

            List<byte[]> sig = fileSignature[ext];
            foreach (byte[] b in sig)
            {
                var curFileSig = new byte[b.Length];
                Array.Copy(fileData, curFileSig, b.Length);
                if (curFileSig.SequenceEqual(b))
                {
                    flag = true;
                    break;
                }
            }

            return flag;
        }
    }
    public static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder UsePort(this IWebHostBuilder builder)
        {
            var port = Environment.GetEnvironmentVariable("PORT");
            if (string.IsNullOrEmpty(port))
            {
                return builder;
            }
            return builder.UseUrls($"https://+:{port}");
        }
    }


    public class ImageManager
    {
        /// <summary>
        /// System.Drawingを使って圧縮された画像のストリームと名前を得る
        /// </summary>
        /// <param name="file">入力画像ファイル</param>
        /// <returns>名前と画像のストリームのタプル</returns>
        private static Tuple<string,MemoryStream> GetCommpressedImage(IFormFile file)
        {
            var imageId = DateTime.Now.ToString("yyyyMMddhhmmss");
            var imageStream = new MemoryStream();

            try
            {
                Bitmap origin = new Bitmap(file.OpenReadStream());
                var rate = 500.0d / Math.Max(origin.Width, origin.Height);
                Bitmap target = new Bitmap(origin, new System.Drawing.Size((int)(origin.Width * rate),(int) (origin.Height * rate)));
                target.Save(imageStream, System.Drawing.Imaging.ImageFormat.Png);
                imageStream.Position = 0;

            }
            catch(Exception e)
            {
                file.OpenReadStream().CopyTo(imageStream);
                Debug.WriteLine(e.Message);
                return new Tuple<string, MemoryStream>(imageId,imageStream);
            }
            return new Tuple<string, MemoryStream>(imageId, imageStream);
        }
        /// <summary>
        /// SixLabor.ImageSharpを使って圧縮された画像のストリームと名前を得る
        /// </summary>
        /// <param name="file">入力画像ファイル</param>
        /// <returns>名前と画像のストリームのタプル</returns>
        private static Tuple<string, MemoryStream> GCIWIS(IFormFile file) {


            SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.Load(file.OpenReadStream());
            var resultSteam = new MemoryStream();
            try
            {
                var bigger = Math.Max(image.Height, image.Width);
                var rate = 500.0d / (double)bigger;
                image.Mutate(x => x.Resize((int)(image.Width * rate), (int)(image.Height * rate)));
                image.SaveAsPng(resultSteam);
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                file.CopyTo(resultSteam);
            }
            finally
            {
                image.Dispose();
            }
            return new Tuple<string, MemoryStream>(file.FileName, resultSteam);








        }



/// <summary>
/// memorystreamをBase64urlに変換
/// </summary>
/// <param name="ms">入力ストリーム</param>
/// <returns>Base64url文字列</returns>


        private static string MsToBase64(MemoryStream ms)
        {
            try
            {
                var contents = ms.ToArray();

                string b64 = Convert.ToBase64String(contents).TrimEnd('=').Replace('+', '-').Replace('/', '_');
                return b64;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        ///　画像を圧縮しデータベースに投入する
        /// </summary>
        /// <param name="file">入力ファイル</param>
        /// <param name="imageModelDbContext">画像データベースにのコンテキスト</param>
        /// <returns>画像データベースのキー</returns>
        public static string WriteToDb(IFormFile file,ImageModelDbContext imageModelDbContext)
        {
            Tuple<string, MemoryStream> tuple=null;
            try {
                 tuple = GCIWIS(file);
            }
            catch(Exception e)
            {
                return e.Message;
            }
             if (tuple.Item2 == null)
            {
                return tuple.Item1;
            }
            var name = tuple.Item1;
            var imageStream = tuple.Item2;
            var imageModel = new ImageModel() { Id = name, image = imageStream.ToArray() };
            imageStream.Dispose();
            try
            {
                imageModelDbContext.Add(imageModel);
                imageModelDbContext.SaveChanges();
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
                return e.Message;
            }
                return name;









        }

    }


}
