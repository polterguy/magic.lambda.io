﻿/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2021, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System.IO;
using System.Threading.Tasks;
using magic.lambda.io.contracts;

namespace magic.lambda.io.stream.services
{
    /// <inheritdoc/>
    public class StreamService : IStreamService
    {
        /// <inheritdoc/>
        public Stream OpenFile(string path)
        {
            return File.OpenRead(path);
        }

        /// <inheritdoc/>
        public void SaveFile(Stream stream, string path)
        {
            using (var fileStream = File.Create(path))
            {
                stream.CopyTo(fileStream);
            }
        }

        /// <inheritdoc/>
        public async Task SaveFileAsync(Stream stream, string path)
        {
            using (var fileStream = File.Create(path))
            {
                await stream.CopyToAsync(fileStream);
            }
        }
    }
}
