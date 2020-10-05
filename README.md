# FileSearch
Easy search for strings in files. Uses KMP matching algorithm and different encodings.

Expects a string and a directory/file path as arguments.
Generates byte patterns for all available (ASCII, UTF-16 LE and BE, UTF-7, UTF-8, UTF-32) encodings and looks for them using Knuth-Morris-Pratt algorithm (implementation taken from: https://github.com/cschen1205/cs-algorithms and slightly modified to work with bytes).
