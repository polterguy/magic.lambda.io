
# Magic Lambda IO

[![Build status](https://travis-ci.org/polterguy/magic.lambda.io.svg?master)](https://travis-ci.org/polterguy/magic.lambda.io)

This project provides file/folder slots for [Magic](https://github.com/polterguy/magic). More specifically, it provides the following slots.

* __[io.folder.create]__ - Creates a folder on disc for you on your server.
* __[io.folder.exists]__ - Returns true if folder exists, otherwise false.
* __[io.folder.delete]__ - Deletes a folder on disc on your server.
* __[io.folder.list]__ - Lists all folders within another source folder.
* __[io.file.load]__ - Loads a file from disc on your server.
* __[io.file.save]__ - Saves a file on disc on your server.
* __[io.file.exists]__ - Returns true if file exists, otherwise false.
* __[io.file.delete]__ - Deletes a file on your server.
* __[io.files.copy]__ - Copies a file on your server.
* __[io.files.execute]__ - Executes a Hyperlambda file on your server.
* __[io.files.list]__ - List files in the specified folder on your server.
* __[io.files.move]__ - Moves a file on your server.
* __[io.content.zip-stream]__ - Creates a ZipStream for you, without touching the file system.
* __[.io.folder.root]__ - Returns the root folder of your system. (private C# slot)

## Fundamentals

All of these slots can _only_ manipulate files and folders inside of your _"files"_ folder in your web server.
This are normally files inside of the folder you have configured in your _"appsettings.json"_ file, with the
key _"magic.io.root-folder"_. This implies that all paths are _relative_ to this path, and no files or folders
from outside of this folder can in any ways be manipulated using these slots.

Notice, if you wish to change this configuration value, then the character tilde "~" means root
folder where your application is running from within. There is nothing preventing you from using an
absolute path, but if you do, you must make sure your web server process have full rights to modify
files within this folder.

### io.folder.create

Creates a new folder on disc. The example below will create a folder named _"foo"_ inside of the _"misc"_ folder.
Notice, will throw an exception if the folder already exists.

```
io.folder.create:/misc/foo/
```

### io.folder.exists

Returns true if specified folder exists on disc.

```
io.folder.exists:/misc/foo/
```

### io.folder.delete

Deletes the specified folder on disc. Notice, will throw an exception if the folder doesn't exists.

```
io.folder.delete:/misc/foo/
```

### io.folder.list

Lists all folders inside of the specified folder. By default hidden folders will not be shown, unless
you pass in **[display-hidden]** and set its value to boolean _"true"_.

```
io.folder.list:/misc/
```

### io.file.load

Loads the specified text file from disc. This slot can _only load text files_. Or to be specific,
there are no ways you can change binary files, hence loading a binary file is for the most parts
not something you should do. Although there _might_ exist exceptions to this.

```
io.file.load:/misc/README.md
```

### io.file.save

Saves the specified content to the specified file on disc, overwriting any previous content if the
file exists from before, creating a new file if no such file already exists. The value of the first
argument will be considered the content of the file.

Notice, the node itself will be evaluated, allowing you to have other slots evaluated before slot saves
the file, to return dynamically the content of your file.

```
io.file.save:/misc/README2.md
   .:This is new content for file
```

### io.file.exists

Returns true if specified file exists from before.

```
io.file.exists:/misc/README.md
```

### io.file.delete

Deletes the specified file. Will throw an exception if the file doesn't exist.

```
io.file.load:/misc/DOES-NOT-EXIST.md
```

### io.files.copy

Copies the specified file to the specified destination folder and file.
Notice, requires the destination folder to exist from before, and the source
file to exist from before. This slot will delete any previously existing destination
file, before starting the copying process. Just like the save slot, this will evaluate
the lambda children before it executes the copying of your file, allowing you to use
the results of slots as the destination path for your file.

```
io.file.copy:/misc/README.md
   .:/misc/backup/README-backup.md
```

### io.files.execute

Executes the specified Hyperlambda file. Just like when evaluating a dynamic slot, you can
pass in an **[.arguments]** node to the file, which will be considered arguments to your file.
Hence, this slot allows you to invoke a file, as if it was a dynamically created slot, and there
is no semantic difference really between this slot and **[slots.signal]** from the _magic.lambda.slots_
project.

```
io.files.execute:/misc/some-hyperlambda-file.hl
```

### io.files.list

Lists all files inside of the specified folder. By default hidden files will not be shown, unless
you pass in **[display-hidden]** and set its value to boolean _"true"_.

```
io.files.list:/misc/
```

### io.files.move

Similar to **[io.files.copy]** but deletes the source file after evaluating.

```
io.files.move:/misc/README.md
   .:/misc/backup/README-backup.md
```

### io.content.zip-stream

Creates a memory based ZIP stream you can return over the response socket connection. Notice,
this doesn't create a zip file, but rather a zip stream, which you can manipulate using other
slots. This slot is useful if you need to return zipped content as your HTTP response for instance.

```
io.content.zip-stream
   .:/foo/x.txt
      .:content of file
```

### .io.folder.root

Returns the root folder of the system. Cannot be invoked from Hyperlambda, but only from C#. Intended as
a support function for other C# slots.

```csharp
var node = new Node();
signaler.Signal(".io.folder.root", node);

// Retrieving root folder after evaluating slot.
var rootFolder = node.Get<string>();
```

## License

Although most of Magic's source code is publicly available, Magic is _not_ Open Source or Free Software.
You have to obtain a valid license key to install it in production, and I normally charge a fee for such a
key. You can [obtain a license key here](https://servergardens.com/buy/).
Notice, 5 hours after you put Magic into production, it will stop functioning, unless you have a valid
license for it.

* [Get licensed](https://servergardens.com/buy/)
