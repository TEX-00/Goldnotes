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
            string ext = Path.GetExtension(fileName);
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



    public class ImageManager
    {
        private static Tuple<string,MemoryStream> GetCommpressedImage(IFormFile file)
        {
            var imageId = DateTime.Now.ToString("yyyyMMddhhmmss");
            var imageStream = new MemoryStream();
            if (!FileChecker.IsValidFile(file))
            {
                return null;
            }
            try
            {
                Bitmap origin = new Bitmap(file.OpenReadStream());
                var rate = 500.0d / Math.Max(origin.Width, origin.Height);
                Bitmap target = new Bitmap(origin, new Size((int)(origin.Width * rate),(int) (origin.Height * rate)));
                target.Save(imageStream, System.Drawing.Imaging.ImageFormat.Png);
                imageStream.Position = 0;

            }
            catch
            {
                imageStream.Dispose();

                return null;
            }
            return new Tuple<string, MemoryStream>(imageId, imageStream);
        }


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
        public static string WriteToDb(IFormFile file,ImageModelDbContext imageModelDbContext)
        {
            var tuple = GetCommpressedImage(file);
            if (tuple == null)
            {
                return null;
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
            catch
            {
                return null;
            }
                return name;









        }

    }


}
