/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using magic.node;
using magic.node.extensions;
using magic.signals.contracts;
using magic.lambda.io.tests.helpers;

namespace magic.lambda.io.tests
{
    public class IOTests
    {
        [Fact]
        public void SaveFile()
        {
            var saveInvoked = false;
            var existsInvoked = false;
            var fileService = new FileService
            {
                SaveAction = (path, content) =>
                {
                    Assert.Equal("foo", content);
                    Assert.Equal(
                        AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/").TrimEnd('/')
                        + "/" +
                        "existing.txt", path);
                    saveInvoked = true;
                },
                ExistsAction = (path) =>
                {
                    existsInvoked = true;
                    Assert.Equal(
                        AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/").TrimEnd('/')
                        + "/" +
                        "existing.txt", path);
                    return true;
                }
            };
            var lambda = Common.Evaluate(@"
io.files.save:existing.txt
   .:foo
io.files.exists:/existing.txt
", fileService);
            Assert.True(saveInvoked);
            Assert.True(existsInvoked);
            Assert.True(lambda.Children.Skip(1).First().Get<bool>());
        }

        [Fact]
        public void SaveAndLoadFile()
        {
            var saveInvoked = false;
            var loadInvoked = false;
            var fileService = new FileService
            {
                SaveAction = (path, content) =>
                {
                    Assert.Equal("foo", content);
                    Assert.Equal(
                        AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/").TrimEnd('/')
                        + "/" +
                        "existing.txt", path);
                    saveInvoked = true;
                },
                LoadAction = (path) =>
                {
                    Assert.Equal(
                        AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/").TrimEnd('/')
                        + "/" +
                        "existing.txt", path);
                    loadInvoked = true;
                    return "foo";
                }
            };
            var lambda = Common.Evaluate(@"
io.files.save:existing.txt
   .:foo
io.file.load:/existing.txt
", fileService);
            Assert.True(saveInvoked);
            Assert.True(loadInvoked);
            Assert.Equal("foo", lambda.Children.Skip(1).First().Get<string>());
        }

        [Fact]
        public async Task SaveAndLoadFileAsync()
        {
            var saveInvoked = false;
            var loadInvoked = false;
            var fileService = new FileService
            {
                SaveAsyncAction = async (path, content) =>
                {
                    Assert.Equal("foo", content);
                    Assert.Equal(
                        AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/").TrimEnd('/')
                        + "/" +
                        "existing.txt", path);
                    saveInvoked = true;
                    await Task.Yield();
                },
                LoadAsyncAction = async (path) =>
                {
                    Assert.Equal(
                        AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/").TrimEnd('/')
                        + "/" +
                        "existing.txt", path);
                    loadInvoked = true;
                    await Task.Yield();
                    return "foo";
                }
            };
            var lambda = await Common.EvaluateAsync(@"
wait.io.files.save:existing.txt
   .:foo
wait.io.file.load:/existing.txt
", fileService);
            Assert.True(saveInvoked);
            Assert.True(loadInvoked);
            Assert.Equal("foo", lambda.Children.Skip(1).First().Get<string>());
        }

        [Fact]
        public void SaveFileAndMove()
        {
            var existsInvoked = 0;
            var saveInvoked = false;
            var moveInvoked = false;
            var fileService = new FileService
            {
                SaveAction = (path, content) =>
                {
                    Assert.Equal("foo", content);
                    Assert.Equal(
                        AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/").TrimEnd('/')
                        + "/" +
                        "existing.txt", path);
                    saveInvoked = true;
                },
                ExistsAction = (path) =>
                {
                    existsInvoked += 1;
                    if (existsInvoked == 1)
                    {
                        Assert.Equal(
                            AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/").TrimEnd('/')
                            + "/" +
                            "moved.txt", path);
                        return false;
                    }
                    else if (existsInvoked == 2)
                    {
                        Assert.Equal(
                            AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/").TrimEnd('/')
                            + "/" +
                            "moved.txt", path);
                        return true;
                    }
                    else if (existsInvoked == 3)
                    {
                        Assert.Equal(
                            AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/").TrimEnd('/')
                            + "/" +
                            "existing.txt", path);
                        return false;
                    }
                    else
                    {
                        throw new Exception("Failure in unit test");
                    }
                },
                MoveAction = (src, dest) =>
                {
                    Assert.Equal(
                        AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/").TrimEnd('/')
                        + "/" +
                        "existing.txt", src);
                    Assert.Equal(
                        AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/").TrimEnd('/')
                        + "/" +
                        "moved.txt", dest);
                    moveInvoked = true;
                }
            };
            var lambda = Common.Evaluate(@"
io.files.save:/existing.txt
   .:foo
io.files.move:/existing.txt
   .:moved.txt
io.files.exists:/moved.txt
io.files.exists:/existing.txt
", fileService);
            Assert.True(saveInvoked);
            Assert.True(moveInvoked);
            Assert.Equal(3, existsInvoked);
            Assert.True(lambda.Children.Skip(2).First().Get<bool>());
            Assert.False(lambda.Children.Skip(3).First().Get<bool>());
        }

        [Fact]
        public void SaveFileAndCopy()
        {
            var existsInvoked = 0;
            var saveInvoked = false;
            var copyInvoked = false;
            var fileService = new FileService
            {
                SaveAction = (path, content) =>
                {
                    Assert.Equal("foo", content);
                    Assert.Equal(
                        AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/").TrimEnd('/')
                        + "/" +
                        "existing.txt", path);
                    saveInvoked = true;
                },
                ExistsAction = (path) =>
                {
                    existsInvoked += 1;
                    if (existsInvoked == 1)
                    {
                        Assert.Equal(
                            AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/").TrimEnd('/')
                            + "/" +
                            "moved.txt", path);
                        return false;
                    }
                    else if (existsInvoked == 2)
                    {
                        Assert.Equal(
                            AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/").TrimEnd('/')
                            + "/" +
                            "moved.txt", path);
                        return true;
                    }
                    else if (existsInvoked == 3)
                    {
                        Assert.Equal(
                            AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/").TrimEnd('/')
                            + "/" +
                            "existing.txt", path);
                        return false;
                    }
                    else
                    {
                        throw new Exception("Failure in unit test");
                    }
                },
                CopyAction = (src, dest) =>
                {
                    Assert.Equal(
                        AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/").TrimEnd('/')
                        + "/" +
                        "existing.txt", src);
                    Assert.Equal(
                        AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/").TrimEnd('/')
                        + "/" +
                        "moved.txt", dest);
                    copyInvoked = true;
                }
            };
            var lambda = Common.Evaluate(@"
io.files.save:/existing.txt
   .:foo
io.files.copy:/existing.txt
   .:moved.txt
io.files.exists:/moved.txt
io.files.exists:/existing.txt
", fileService);
            Assert.True(saveInvoked);
            Assert.True(copyInvoked);
            Assert.Equal(3, existsInvoked);
            Assert.True(lambda.Children.Skip(2).First().Get<bool>());
            Assert.False(lambda.Children.Skip(3).First().Get<bool>());
        }

        [Fact]
        public async Task SaveFileAndCopyAsync()
        {
            var existsInvoked = 0;
            var saveInvoked = false;
            var copyInvoked = false;
            var fileService = new FileService
            {
                SaveAction = (path, content) =>
                {
                    Assert.Equal("foo", content);
                    Assert.Equal(
                        AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/").TrimEnd('/')
                        + "/" +
                        "existing.txt", path);
                    saveInvoked = true;
                },
                ExistsAction = (path) =>
                {
                    existsInvoked += 1;
                    if (existsInvoked == 1)
                    {
                        Assert.Equal(
                            AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/").TrimEnd('/')
                            + "/" +
                            "moved.txt", path);
                        return false;
                    }
                    else if (existsInvoked == 2)
                    {
                        Assert.Equal(
                            AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/").TrimEnd('/')
                            + "/" +
                            "moved.txt", path);
                        return true;
                    }
                    else if (existsInvoked == 3)
                    {
                        Assert.Equal(
                            AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/").TrimEnd('/')
                            + "/" +
                            "existing.txt", path);
                        return false;
                    }
                    else
                    {
                        throw new Exception("Failure in unit test");
                    }
                },
                CopyAsyncAction = async (src, dest) =>
                {
                    Assert.Equal(
                        AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/").TrimEnd('/')
                        + "/" +
                        "existing.txt", src);
                    Assert.Equal(
                        AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/").TrimEnd('/')
                        + "/" +
                        "moved.txt", dest);
                    copyInvoked = true;
                    await Task.Yield();
                }
            };
            var lambda = await Common.EvaluateAsync(@"
io.files.save:/existing.txt
   .:foo
wait.io.files.copy:/existing.txt
   .:moved.txt
io.files.exists:/moved.txt
io.files.exists:/existing.txt
", fileService);
            Assert.True(saveInvoked);
            Assert.True(copyInvoked);
            Assert.Equal(3, existsInvoked);
            Assert.True(lambda.Children.Skip(2).First().Get<bool>());
            Assert.False(lambda.Children.Skip(3).First().Get<bool>());
        }

        [Slot(Name = "foo")]
        public class EventSource : ISlot
        {
            public void Signal(ISignaler signaler, Node input)
            {
                input.Value = "success";
            }
        }

        [Fact]
        public void SaveWithEventSourceAndLoadFile()
        {
            var saveInvoked = false;
            var loadInvoked = false;
            var fileService = new FileService
            {
                SaveAction = (path, content) =>
                {
                    Assert.Equal("success", content);
                    Assert.Equal(
                        AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/").TrimEnd('/')
                        + "/" +
                        "existing.txt", path);
                    saveInvoked = true;
                },
                LoadAction = (path) =>
                {
                    Assert.Equal(
                        AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/").TrimEnd('/')
                        + "/" +
                        "existing.txt", path);
                    loadInvoked = true;
                    return "success";
                }
            };
            var lambda = Common.Evaluate(@"
io.files.save:existing.txt
   foo:error
io.file.load:/existing.txt
", fileService);
            Assert.True(saveInvoked);
            Assert.True(loadInvoked);
            Assert.Equal("success", lambda.Children.Skip(1).First().Get<string>());
        }

        [Fact]
        public void SaveOverwriteAndLoadFile()
        {
            var saveInvoked = 0;
            var loadInvoked = false;
            var fileService = new FileService
            {
                SaveAction = (path, content) =>
                {
                    saveInvoked += 1;
                    if (saveInvoked == 1)
                        Assert.Equal("foo", content);
                    else if (saveInvoked == 2)
                        Assert.Equal("foo1", content);
                    else
                        throw new Exception("Unit test failure");
                    Assert.Equal(
                        AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/").TrimEnd('/')
                        + "/" +
                        "existing.txt", path);
                },
                LoadAction = (path) =>
                {
                    Assert.Equal(
                        AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/").TrimEnd('/')
                        + "/" +
                        "existing.txt", path);
                    loadInvoked = true;
                    return "foo1";
                }
            };
            var lambda = Common.Evaluate(@"
io.files.save:existing.txt
   .:foo
io.files.save:existing.txt
   .:foo1
io.file.load:/existing.txt
", fileService);
            Assert.Equal(2, saveInvoked);
            Assert.True(loadInvoked);
            Assert.Equal("foo1", lambda.Children.Skip(2).First().Get<string>());
        }

        [Fact]
        public void ListFiles()
        {
            var fileService = new FileService
            {
                ListFilesAction = (path) =>
                {
                    return new string[] {
                        AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/").TrimEnd('/')
                        + "/" +
                        "foo.txt",
                        AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/").TrimEnd('/')
                        + "/" +
                        "bar.txt"
                    };
                }
            };
            var lambda = Common.Evaluate(@"
io.files.list:/
", fileService);
            Assert.True(lambda.Children.First().Children.Count() == 2);

            // Notice, files are SORTED!
            Assert.Equal("/bar.txt", lambda.Children.First().Children.First().Get<string>());
            Assert.Equal("/foo.txt", lambda.Children.First().Children.Skip(1).First().Get<string>());
        }

        [Fact]
        public void ListFolders()
        {
            var listInvoked = false;
            var folderService = new FolderService
            {
                ListAction = (path) =>
                {
                    listInvoked = true;
                    return new string[] {
                        AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/").TrimEnd('/')
                        + "/" +
                        "foo/",
                        AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/").TrimEnd('/')
                        + "/" +
                        "bar/"
                    };
                }
            };
            var lambda = Common.Evaluate(@"
io.folder.list:/
", null, folderService);
            Assert.True(listInvoked);
            Assert.True(lambda.Children.First().Children.Count() == 2);

            // Notice, files are SORTED!
            Assert.Equal("/bar/", lambda.Children.First().Children.First().Get<string>());
            Assert.Equal("/foo/", lambda.Children.First().Children.Skip(1).First().Get<string>());
        }

        [Fact]
        public void CreateFolderListFolders()
        {
            var listInvoked = false;
            var createInvoked = true;
            var folderService = new FolderService
            {
                CreateAction = (path) =>
                {
                    createInvoked = true;
                    Assert.Equal(
                        AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/").TrimEnd('/')
                        + "/" +
                        "foo", path);
                },
                ListAction = (path) =>
                {
                    listInvoked = true;
                    return new string[] {
                        AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/").TrimEnd('/')
                        + "/" +
                        "foo/",
                        AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/").TrimEnd('/')
                        + "/" +
                        "bar/"
                    };
                }
            };
            var lambda = Common.Evaluate(@"
io.folder.create:/foo
io.folder.list:/
", null, folderService);
            Assert.True(listInvoked);
            Assert.True(createInvoked);
            Assert.True(lambda.Children.Skip(1).First().Children.Count() == 2);

            // Notice, files are SORTED!
            Assert.Equal("/bar/", lambda.Children.Skip(1).First().Children.First().Get<string>());
            Assert.Equal("/foo/", lambda.Children.Skip(1).First().Children.Skip(1).First().Get<string>());
        }

        [Fact]
        public void EvaluateFile()
        {
            var loadInvoked = false;
            var fileService = new FileService
            {
                LoadAction = (path) =>
                {
                    Assert.Equal(
                        AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/").TrimEnd('/')
                        + "/" +
                        "foo.hl", path);
                    loadInvoked = true;
                    return @"slots.return-nodes
   result:hello world";
                }
            };
            var lambda = Common.Evaluate(@"
io.files.eval:foo.hl
", fileService);
            Assert.True(loadInvoked);
            Assert.Single(lambda.Children.First().Children);
            Assert.Equal("hello world", lambda.Children.First().Children.First().Get<string>());
        }

        [Fact]
        public void EvaluateFileWithArguments()
        {
            var loadInvoked = false;
            var fileService = new FileService
            {
                LoadAction = (path) =>
                {
                    Assert.Equal(
                        AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/").TrimEnd('/')
                        + "/" +
                        "foo.hl", path);
                    loadInvoked = true;
                    return @"unwrap:x:+/*
slots.return-nodes
   result:x:@.arguments/*";
                }
            };
            var lambda = Common.Evaluate(@"
io.files.eval:foo.hl
   input:jo world
", fileService);
            Assert.True(loadInvoked);
            Assert.Single(lambda.Children.First().Children);
            Assert.Equal("result", lambda.Children.First().Children.First().Name);
            Assert.Equal("jo world", lambda.Children.First().Children.First().Get<string>());
        }

        [Fact]
        public void EvaluateFileReturningValue()
        {
            var loadInvoked = false;
            var fileService = new FileService
            {
                LoadAction = (path) =>
                {
                    Assert.Equal(
                        AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/").TrimEnd('/')
                        + "/" +
                        "foo.hl", path);
                    loadInvoked = true;
                    return "slots.return-value:howdy world";
                }
            };
            var lambda = Common.Evaluate("io.files.eval:foo.hl", fileService);
            Assert.True(loadInvoked);
            Assert.Empty(lambda.Children.First().Children);
            Assert.Equal("howdy world", lambda.Children.First().Get<string>());
        }
    }
}
