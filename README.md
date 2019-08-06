# Azure Storage Functions

## Requirements

- .Net Core 2.1
- Storage Emulator
- Storage Explorer

## Project MoveBlob

An Azure function that can perform a move (copies the original file to another storage account and then deletes the original blob) from one storage account to another.

## Project: ProcessBlob And Encoder

Enconder is a .Net Console app that can serialize a Contact POCO object first to JSON, then to base64 encoding and generates a file.

ProcessBlob is an Azure function that can deserialize from blob, to base64, to JSON, and save the JSON to a new container and then move the original blob to an archive container.

### Testing Locally

- Start the Storage emulator
- Generate a couple of files using the Encoder console application. If you run from Visual Studio these files will be location on the bin\debug folder and they end with .b64
- Start the VS and debug into the ProcessExplorer Blob. Put a breakpoint at ProcessAsync()
- Upload a couple of the generated .b64 files to using the Storage explorer to the process-container
- The expected output should be JSON files in the json-container, no files in the process-container, and the original files in the archive-container.
