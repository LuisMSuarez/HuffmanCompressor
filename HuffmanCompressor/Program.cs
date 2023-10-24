// See https://aka.ms/new-console-template for more information
using HuffmanCompressor;

IFileCompressor compressor = new HuffmanCompressor.HuffmanCompressor();
compressor.Compress(@".\input.txt", @".\output.bin");
Console.WriteLine("Hello, World!");
