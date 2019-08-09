# Azure Storage Functions

## Introduction

These are a couple of sample Azure Functions to demo the ability to move blobs from one storage account to another and to process a blob and then move it to an archival container.

## Requirements

- .Net Core 2.1
- Storage Emulator
  - https://docs.microsoft.com/en-us/azure/storage/common/storage-use-emulator
- Storage Explorer
  - https://azure.microsoft.com/en-us/features/storage-explorer/

## Project MoveBlob

- An Azure function that can perform a move (copies the original file to another storage account and then deletes the original blob) from one storage account to another.

## Projects: ProcessBlob And Encoder

- Enconder is a .Net Console app that can serialize a Contact POCO object first to JSON, then to base64 encoding and then generates a file ending in .b64.
 
- ProcessBlob is an Azure function that can deserialize from blob, to base64, to JSON, and save the JSON to a new container and then move the original blob to an archive container.

### Testing Locally

- Start the Storage emulator
- Generate a couple of encoded files using the Encoder console application. 
  - If you run from Visual Studio these files will be located on the bin\debug folder and they end with .b64
- Start the VS and debug into the ProcessExplorer Blob. Put a breakpoint at ProcessAsync()
- Start the Storage Emulator and create a process-container
- Upload a couple of the generated .b64 files to the storage emulator using the Storage explorer to the process-container
- Return into VS and step into the code
  - The expected output should be JSON files in the json-container, no files in the process-container, and the original files in the archive-container.
