﻿/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2021, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip;
using magic.node;
using magic.node.extensions;
using magic.signals.contracts;

namespace magic.lambda.io.file
{
    /// <summary>
    /// [io.content.zip-stream] slot for zipping a bunch of files into a specified stream.
    /// </summary>
    [Slot(Name = "io.content.zip-stream")]
    public class ZipContent : ISlot
    {
        /// <summary>
        /// Implementation of slot.
        /// </summary>
        /// <param name="signaler">Signaler used to raise the signal.</param>
        /// <param name="input">Arguments to slot.</param>
        public void Signal(ISignaler signaler, Node input)
        {
            // Evaluating all filenames, in case they're slot invocations.
            signaler.Signal("eval", input);

            // Notice, this stream is returned to caller, and never disposed - Which is its entire purpose!
            var result = new MemoryStream();

            // Creating Zip archive.
            using (var zipStream = new ZipOutputStream(result))
            {
                zipStream.IsStreamOwner = false;
                var writer = new StreamWriter(zipStream);

                // Iterating through each entity caller wants to zip, and creating entry for item.
                foreach (var idx in input.Children)
                {
                    // Evaluating content node, in case it's a slot invocation.
                    signaler.Signal("eval", idx);

                    // Creating currently iterated Zip entry.
                    var newEntry = new ZipEntry(ZipEntry.CleanName(idx.GetEx<string>()))
                    {
                        DateTime = DateTime.Now
                    };
                    var content = idx.Children.FirstOrDefault()?.GetEx<string>() ?? "";
                    zipStream.PutNextEntry(newEntry);
                    writer.Write(content);
                    writer.Flush();
                }
            }

            // Important! Such that caller can use stream directly, read from it, copy it, etc - Without having to fiddle with it first.
            result.Position = 0;
            input.Value = result;
        }
    }
}
