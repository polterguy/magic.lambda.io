﻿/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System.Text;
using System.IO;
using System.Threading.Tasks;
using magic.lambda.io.contracts;
using System.Collections.Generic;

namespace magic.lambda.io.file.services
{
    public class FileService : IFileService
    {
        public void Copy(string source, string destination)
        {
            File.Copy(source, destination);
        }

        public async Task CopyAsync(string source, string destination)
        {
            using (Stream sourceStream = File.OpenRead(source))
            {
                using (Stream destinationStream = File.Create(destination))
                {
                    await sourceStream.CopyToAsync(destinationStream);
                }
            }
        }

        public void Delete(string path)
        {
            File.Delete(path);
        }

        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public IEnumerable<string> ListFiles(string folder)
        {
            return Directory.GetFiles(folder);
        }

        public string Load(string path)
        {
            return File.ReadAllText(path, Encoding.UTF8);
        }

        public async Task<string> LoadAsync(string path)
        {
            using (var file = File.OpenText(path))
            {
                return await file.ReadToEndAsync();
            }
        }

        public void Move(string source, string destination)
        {
            File.Move(source, destination);
        }

        public void Save(string filename, string content)
        {
            File.WriteAllText(filename, content);;
        }

        public async Task SaveAsync(string filename, string content)
        {
            using (var writer = File.CreateText(filename))
            {
                await writer.WriteAsync(content);
            }
        }
    }
}