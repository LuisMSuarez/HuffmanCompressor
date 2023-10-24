// See https://aka.ms/new-console-template for more information
using HuffmanCompressor;

IFileCompressor compressor = new HuffmanCompressor.HuffmanCompressor();
// compressor.Compress(@".\input.txt", @".\output.bin");
compressor.Decompress(@".\output.bin", @".\decompressed.txt");
Console.WriteLine("Hello, World!");
