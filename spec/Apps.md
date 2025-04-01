# about
Provides help information for Link package commands.
Usage: about [appName] [--h]
* appName: Command on which to display information. If this parameter is not provided, all the commands available in the package are listed.
* --h: Displays this help and exits.

# append
Adds a string to the end of each input line.
Usage: append suffix [--h]
* suffix: The string to append to each input line.
* --h: Displays this help and exits.

# copyFiles
Copies the input files to another location. Unless they already exist, all directories specified in the destinations will be created. The command returns the copied files.
Usage: copyFiles pattern dest [src] [--ri][--ovw][--cs][--h]
* pattern: Represents a regular expression pattern to match. The input lines for which 'pattern' is not matched are ignored.
* dest: A transform pattern to apply to each input line to extract the destination file. It can consist of any combination of literal text and substitutions based on 'pattern', such as capturing group that is identified by a number or a name.
* src: A transform pattern to apply to each input line to extract the source file. It can consist of any combination of literal text and substitutions based on 'pattern', such as capturing group that is identified by a number or a name. By default, the input line is considered the source file.
* --ri: For each file copied, returns its input line. If this switch is omitted, the command returns the destination file.
* --ovw: Destination file should be replaced if it already exists.
* --cs: Specifies a case-sensitive matching.
* --h: Displays this help and exits.

# copyToDir
Copies existing input files to another directory. The command returns the copied files.
Usage: copyToDir dest [--ri][--ovw][--h]
* dest: The name of the directory to copy to. All directories specified in 'dest' will be created, unless they already exist.
* --ri: For each file copied, returns its input line instead of the destination file.
* --ovw: Destination file should be replaced if it already exists.
* --h: Displays this help and exits.

# count
Counts the number of input lines.
Usage: count [--h]
* --h: Displays this help and exits.

# countFrq
Returns the number of occurrences of each character, or word, that appears in the input text.
Usage: countFrq [--wrd][--frq [precision]][--sep separator][--h]
* --wrd: Counts words instead of characters.
* --frq: Displays the  frequency of each item. 'precision' is the precision specifier, the number of fractional digits to display for the frequency value. The default is to display 4 digits after the decimal point.
* --sep: Uses 'separator' as a string separator between each item and its count. If 'separator' is missing, a colon is used.
* --h: Displays this help and exits.

# createDirs
Creates all directories and sub-directories specified in the input path unless they already exist. The command returns the created input directories.
Usage: createDirs [--rx ptrn dir [--cs][--ri]] [--h]
* --rx: Uses a regular expression to extract the target directory from the input lines. ‘ptrn’ represents a regular expression pattern to match. Input lines for which ‘ptrn’ is not matched are ignored. ‘dir’ is the directory to be created. It can consist of any combination of capturing group that is identified by a number or a name. By default, the input line is considered the target directory.
* --cs: Specifies a case-sensitive matching. It's an error to specify this option without the '--rx' switch.
* --ri: For each directory created, returns its input line instead of the destination directory. It's an error to specify this option without the '--rx' switch. 
* --h: Displays this help and exits.

# delDirs
Deletes directories specified in input path. Returns the sequences of deleted directories. Non-existent directories are ignored.
Usage: delDirs [--rx ptrn dir [--cs][--ri]] [--h]
* --rx: Uses an optional regular expression to extract, from the input line, the target directory. 'ptrn' represents a regular expression pattern to match against each input line. The input line for which ‘ptrn’ is not matched are ignored. 'dir', the directory to delete, can consist of any combination of capturing group that is identified by a number or a name. Any file and sub-directory of the target directory will be also deleted.
* --cs: Specifies a case-sensitive matching. It's an error to specify this option without the '--rx' switch.
* --ri: For each directory deleted, returns its input line instead of the target directory. It's an error to specify this option without the '--rx' switch.
* --h: Displays this help and exits.

# delFiles
Deletes the specified input files. Return the successfully deleted files. Non-existent files are ignored.
Type: Mutator.
Usage: delFiles [--rx ptrn file [--cs]][--ri]] [--h]
* --rx: Uses an optional regular expression to extract, from the input line, the target file. ‘ptrn’ represents a regular expression pattern to match against each input line.
*ptrn: Represents a regular expression pattern to match against each input line. The input line for which ‘ptrn’ is not matched are ignored.
* file: The file to delete.  It can consist of any combination of capturing group that is identified by a number or a name.
* --cs: Specifies a case-sensitive matching. It's an error to specify this option without the '--rx' switch.
* --ri: For each file deleted, returns its input line instead of the target file. It's an error to specify this option without the '--rx' switch.
* --h: Displays this help and exits.

# dirName
Returns the directory information for each input	 path. In most cases, the string returned consists of all characters in the path up to, but not including, the last directory separator. The file or directory specified by the input path is not required to exist.
Type: Mutator.
Usage: dirName [--rx ptrn path [--cs]] [--pi [sep]][--h]
* --rx: Use regular expression to extract, from the input line, the target path. ‘ptrn’ represents a regular expression pattern to match against each input line. The input lines for which ‘ptrn’ does not match are ignored. ‘path’ is a transform pattern to apply to each input line to extract the target path. It can consist of any combination of literal text and substitutions based on 'ptrn', such as capturing group that is identified by a number or a name.
* --cs: Specifies a case-sensitive matching. It's an error to specify this option without the '--rx' switch.
* --pi: For each result, prepend its input line, using 'sep' as separator. If 'sep' is missing, a colon is used.
* --h: Displays this help and exits.

# dirSize
Returns the size of the contents of each input directory.
Type: Mutator.
Usage: dirSize [--rx ptrn dir [--cs]] [--pi [sep]][--fmt][--h]
* --rx: Use regular expression to extract, from the input line, the target directory.  ‘ptrn’ represents a regular expression pattern to match against each input line. The input lines for which ‘ptrn’ does not match are ignored. ‘dir’ is a transform pattern to apply to each input line to extract the target directory.  It can consist of any combination of capturing group that is identified by a number or a name.
* --cs: Specifies a case-sensitive matching. It's an error to specify this option without the '--rx' switch.
* --pi: For each result, prepend its input line, using 'sep' as separator. If 'sep' is missing, a colon is used.
* --fmt: Returns the size in a human-readable format. If this switch is missing, the size is returned in bytes.
* --h: Displays this help and exits.

# distinct
Remove duplicate lines from input.
Type: Filter.
Usage: distinct [--rx ptrn key] [--cs] [--h]
* --rx: Use regular expression to distinguish input lines. ‘ptrn’ represents a regular expression pattern to match. ‘key’ is the key used to distinguish input lines,  It can consist of any combination of capturing group that is identified by a number or a name. The input lines for which ‘ptrn’ does not match are ignored.
* --cs: Specifies a case-sensitive comparison and/or pattern matching.
* --h: Displays this help and exits.

# dump
Writes its input to the standard output and to a file.
Usage: dump file [--rx ptrn [--cs]][--ovw][--h]
* file: File to which input is written. 
* --rx: Uses regular expression to filter written lines to the specified file. ‘ptrn’ represents a regular expression pattern to match. The input lines for which ‘ptrn’ does not match are not written to the file.
* --cs: Specifies a case-sensitive matching. It's an error to specify this option without the '--rx' switch. 
* --ovw: Overwites the output to each existing file, rather than appending it.
* --h:  Displays this help and exits.

# emit
Returns the contents of each input file.
Type: Mutator.
Usage: emit [--rx ptrn file[--cs]][--h]
* --rx: Use regular expression to extract, from each input line, the target file. ‘ptrn’ represents a regular expression pattern to match against each input line. The input lines for which ‘ptrn’ does not match are ignored. ‘file’ is the file whose contents are to be output.  It can consist of any combination of capturing group that is identified by a number or a name.
* --cs: Specifies a case-sensitive matching. It's an error to specify this option without the '--rx' switch.
* --h: Displays this help and exits.

# endsWith
Returns input lines that ends with a specified string.
Type: Filter.
Usage: endsWith suffix [--cs][--h]
* suffix: The string to compare to the sub-string at the end of the input line.
* --cs: Specifies a case-sensitive comparison.
* --h: Displays this help and exits.

# enumDirs
Lists subdirectories of a specified directories.
Type: Generator.
Usage: enumDirs [dir_0 dir_1 ...] [--r [depth]][--hid][--dp][--h]
* dir_i: Directory whose contents should be listed. If no directory is specifiled, the contents of the current folder are listed.
* --r: List sub-directories recursively. 'depth' is the depth of the subfolders to be explored. If 'depth' is missing, all subfolders are explored.
* --hid: Includes hidden folders. The default is to not include hidden folders.
* --dp: Discards the path from each subfolder and returns only the subfolder name.
* --h: Displays this help and exits.

# enumFiles
Lists files contained in a specified directories.
Type: Generator.
Usage: enumFiles [dir_0 dir_1 ...] [--r [depth]][--hid][--dp][--h]
* dir_i: Directory whose contents should be listed. If no directory is specifiled, the contents of the current folder are listed.
* --hid: Includes hidden files. The default is to not include hidden files.
* --r: Explore sub-directories content recursively. 'depth' is the depth of the subfolders to be explored. If 'depth' is missing, all subfolders are explored.
* --dp: Discards the path from each file and returns only the file name.
* --h: Displays this help and exits.

# fileExt
Returns the extension (including the period ".") of every input file path. The file need not exist.
Type: Mutator.
Usage: fileExt [--rx ptrn file [--cs]][--pi [sep]][--h]
* --rx: Use regular expression to extract, from the input line, the target file. ‘ptrn’ represents a regular expression pattern to match against each input line. The input lines for which ‘ptrn’ does not match are ignored. ‘file’ is a transform pattern to apply to each input line to extract the target file path. It can consist of any combination of literal text and substitutions based on 'ptrn', such as capturing group that is identified by a number or a name.
* --cs: Specifies a case-sensitive matching. It's an error to specify this option without the '--rx' switch.
* --pi: For each result, prepend its input line, using 'sep' as separator. If 'sep' is missing, a colon is used.
* --h: Displays this help and exits.

# fileName
For every input file,  returns the file name and extension. The path need not exist.
Type: Mutator.
Usage: fileName [--rx ptrn path [--cs]] [--noext][--pi [sep]][--h]
* --rx: Use regular expression to extract, from the input line, the target file. ‘ptrn’ represents a regular expression pattern to match against each input line. The input lines for which ‘ptrn’ does not match are ignored. ‘path’ is a transform pattern to apply to each input line to extract the target file. It can consist of any combination of literal text and substitutions based on 'ptrn', such as capturing group that is identified by a number or a name.
* --cs: Specifies a case-sensitive matching. It's an error to specify this option without the '--rx' switch.
* --noext: Returns the file name without the extension.
* --pi: For each result, prepend its input line, using 'sep' as separator. If 'sep' is missing, a colon is used.
* --h: Displays this help and exits.

# fileSize
Returns the size of each input file.
Type: Mutator.
Usage: fileSize [--rx ptrn file [--cs]][--pi [sep]][--fmt][--h]
* --rx: Use regular expression to extract, from the input line, the target file.  ‘ptrn’ represents a regular expression pattern to match against each input line. The input lines for which ‘ptrn’ does not match are ignored. ‘file’ is a transform pattern to apply to each input line to extract the target file. It can consist of any combination of literal text and substitutions based on 'ptrn', such as capturing group that is identified by a number or a name.
* --cs: Specifies a case-sensitive matching. It's an error to specify this option without the '--rx' switch.
* --pi: For each result, prepend its input line, using 'sep' as separator. If 'sep' is missing, a colon is used.
* --fmt: Returns the size in a human-readable format. If this switch is missing, the size is returned in bytes.
* --h: Displays this help and exits.

# fullPath
Returns the absolute path for every input file or directory. The path need not exist.
Type: Mutator.
Usage: fullPath [--rx ptrn path [--cs]] [--pi [sep]][--h]
* --rx: Use regular expression to extract, from the input line, the target path.  ‘ptrn’ represents a regular expression pattern to match against each input line. The input lines for which ‘ptrn’ does match are ignored. ‘path’ is a transform pattern to apply to each input line to extract the target path. It can consist of any combination of literal text and substitutions based on 'ptrn', such as capturing group that is identified by a number or a name.
* --cs: Specifies a case-sensitive matching. It's an error to specify this option without the '--rx' switch.
* --pi: For each result, prepend its input line, using 'sep' as separator. If 'sep' is missing, a colon is used.
* --h: Displays this help and exits.

# groupBy
Groups the elements of the input sequence according to a specified key.
Type: Mutator.
Usage: groupBy ptrn key [--cs][--h]
* ptrn: Represents a regular expression pattern to match. The input lines for which ‘ptrn’ does not match are ignored.
* key: The key used to group input lines.  This is a transform pattern to apply to each input line to extract the key. It can consist of any combination of literal text and substitutions based on 'ptrn', such as capturing group that is identified by a number or a name.
* --cs: Specifies a case-sensitive pattern matching.
* --h: Displays this help and exits.

# insert
Returns, for each input line, a new line in which a specified string is inserted at a specified index.
Type: Mutator.
Usage: insert str ndx [--pi [sep]][--h]
* str: The string to insert.
* ndx: The zero-based index position of the insertion. Any input line for which 'ndx' is greater then its length is ignored.
* --pi: For each result, prepend its input line, using 'sep' as separator. If 'sep' is missing, a colon is used.
* --h: Displays this help and exits.
* 
# join
Produces a sequence with lines from the input sequence and those from one or more specified files. The command merges each line of the input sequence with lines having the same index in the specified files. The command merges sequences until it reaches the end of one of them. 
Type: Mutator
Usage: join file_0 [file_1 file_2 ...][--sep str][--h]
* file_i: File from which lines to merge are read.
* --sep: Uses 'str' as separator between joined lines. If 'str' is missing, a colon is used as separator.
* --h: Displays this help and exits.

# length
Returns the number of characters for each input line.
Type: Mutator.
Usage: length [--rx ptrn str [--cs]][--pi [sep]][--h]
* --rx: Use regular expression to extract, from the input line, the target string.  ‘ptrn’ represents a regular expression pattern to match against each input line. The input lines for which ‘ptrn’ does not match are ignored. ‘str’ is a transform pattern to apply to each input line to extract the target string. It can consist of any combination of literal text and substitutions based on 'ptrn', such as capturing group that is identified by a number or a name.
* --cs: Specifies a case-sensitive matching. It's an error to specify this option without the '--rx' switch.
* --pi: For each result, prepend its input line, using 'sep' as separator. If 'sep' is missing, a colon is used.
* --h: Displays this help and exits.

# max
Returns the maximum entry in the input sequence.
Type: Consumer
Usage: max [--rx ptrn key] [--cs][--h]
* --rx: Use regular expression to extract, from the input line, the target string. 'ptrn', represents a regular expression pattern to be matched. The input lines for which ‘ptrn’ does not match are ignored. 'key' represents the key used to compare input lines by. It can consist of any combination of capturing group that is identified by a number or a name. 
* --cs: Specifies case-sensitive comparison and/or pattern matching.
* --h: Displays this help and exits.

# merge
Concatenates all the input lines, using the specified separator between each line.
Type: Mutator.
Usage: merge [sep] [--n count][--h]
* sep: The string to use as a separator. If ‘sep’ is missing, an colon is used as separator.
* --n: Merges every ‘count’ input lines. If this flag is missing, all the input lines are merged together. 'count',  a positive integer, represents the number of successive lines to be merged together.
* --h: Displays this help and exits.

# min
Returns the minimum entry in the input sequence.
Type: Consumer
Usage: min [--rx ptrn key][--cs][--h]
* --rx: Use regular expression to extract, from the input line, the target string. 'ptrn', represents a regular expression pattern to be matched. The input lines for which ‘ptrn’ does not match are ignored. 'key' represents the key used to compare input lines by. It can consist of any combination of capturing group that is identified by a number or a name.
* --cs: Specifies case-sensitive comparison and/or pattern matching.
* --h: Displays this help and exits.

# moveToDir
Moves input files to a specified directory. Returns successfully moved files.
Type: Action.
Usage: moveToDir dir [--ri][--ovw][--h]
* dir: The destinition directory. All directories specified in 'dir' will be created, unless they already exist.
* --ri: For each file moved, returns its input line instead of the destination file.
* --ovw: Destination file should be replaced if it already exists.
* --h: Displays this help and exits.

# padEnd
For each input line, returns a new string of a specified length in which the end is padded with a specified character. This command pads the end of the target string. This means that, when used with right-to-left languages, it pads the left portion of the string.
Type: Mutator.
Usage: padEnd len [--c char][--h]
* len: The number of characters in the resulting string, equal to the number of original characters plus any additional padding characters. If 'len' is less than or equal to the original length of the target string, the line is returned unchanged.
* --c: Uses 'char' as padding character. If this flag is missing, a space is used.
* --h: Displays this help and exits.

# padStart
Returns, for each input line, a new string of a specified length in which the beginning of the input line is padded with spaces or with a specified character. This command pads the beginning of the returned string. This means that, when used with right-to-left languages, it pads the right portion of the string.
Type: Mutator.
Usage: padStart len [--c char][--h]
* len: The number of characters in the resulting string, equal to the number of original characters plus any additional padding characters. If 'len' is less than or equal to the length of the input line, the line is returned unchanged.
* --c: Uses 'char' as padding character. If this flag is missing, a space is used.
* --h: Display this help and exits.

# pathDate
Returns the date and time the specified file or directory was last written to.  The result is in ISO 8601 extended format (YYYY-MM-DDThh:mm:ss). Non-existent input paths are ignored.
Type: Mutator.
Usage: pathDate [--rx ptrn path [--cs]] [--crd][--utc][--pi [sep]][--notm][--h]
* --rx: Use regular expression to extract, from the input line, the target path. 'ptrn' represents the regular expression pattern to match against each input line. The input lines for which ‘ptrn’ does not match are ignored. 'path' is a transform pattern to apply to each input line to extract the file or directory for which the date is to seek for. It can consist of any combination of capturing group that is identified by a number or a name.
* --cs: Specifies a case-sensitive matching. It's an error to specify this option without the '--rx' switch.
* --crd: Returns the creation date instead of the modification date.
* --utc:  Returns the date in Coordinated Universal Time.
* --pi: For each result, prepends its input line, using 'sep' as separator. If 'sep' is missing, a colon is used.
* --notm: Returns the date component only.
* --h: Displays this help and exits.

# prepend
Adds a string to the beginning of each input line.
Type: Mutator.
Usage: prepend prefix [--h]
* prefix: The string to prepend to each input line.
* --h: Displays this help and exits.

# project
Projects every input line into a new form.
Type: Mutator.
Usage: project ptrn output [--cs][--h]
* ptrn: Represents a regular expression pattern to match. The input strings for which ‘ptrn’ does not match are ignored.
* output: A transform pattern to apply to each input string. It can consist of any combination of literal text and substitutions, such as capturing group that is identified by a number or a name.
* --cs: Specifies case-sensitive matching.
* --h: Displays this help and exits.

# putAt
Returns the input sequence to which a specified string has been inserted at a specified index.
Type:Mutator
Usage: putAt str ndx [--h]
* str: The string to insert.
* ndx: The zero-based index at which 'str' should be inserted. It is a error to specify an index greater than the length of sequences.
* --h: Displays this help and exits.

# putFirst
Returns the input sequence to which a specified string has been added at the beginning.
Type: Mutator
Usage: putFirst str [--h]
* str: The string to prepend to the input sequence.
* --h: Displays this help and exits.

# putLast
Returns the input sequence to which a specified string has been added at the end.
Type: Mutator
Usage: putlast str [--h]
* str: The string to append to the input sequence.
* --h: Displays this help and exits.

# putWhen
Returns the lines of the input sequence until a specified pattern is matched, then inserts a specified string and returns the remaining lines.
Type: Mutator
Usage: putWhen ptrn str [--cs][--h]
* ptrn: A regular expression pattern to match.
* str: The string to insert.
* --cs: Specifies case-sensitive matching.
* --h: Displays this help and exits.

# randomizeCase
For each input line, each character gets a random case.
Type: Mutator.
Usage: randomizeCase [--h]
* --h: Display this help and exits.

# range
Generates a sequence of integral numbers.
Type: Generator.
Usage: intRange count [--init start][--stp step][--pad][--h]
* count: A nonnegative integer value that represent the number of sequential integers to generate.
* --init: Specifes the value of the first integer in the sequence. 'start', the initial value,  must be greater than or equal to -2147483648 and 'start' + 'count' * 'step' must be less than or equal to 2147483647. The default value is 0.
* --stp: Specifes an optional increment. The default is 1. 'step', the increment, can be any non-zero integer number as long as the value 'start' + 'count' * 'step' is in the interval [-2147483648, 2147483647].
* --pad: Pads the inserted number with leading zeros according to the longest inserted number.
* --h: Displays this help and exits.

# relativePath
Returns, for each input path, a relative path to a specified directory. Returns input line if the paths don't share the same root.
Type: Mutator.
Usage: relativePath dir [--rx ptrn path [--cs]] [--pi [sep]][--h]
* dir: The path the result should be relative to. This parameter is always considered to be a directory.
* --rx: Uses regular expression to extract, from the input line, the target path.  ‘ptrn’ represents a regular expression pattern to match against each input line. The input lines for which ‘ptrn’ does not match are ignored. ‘path’ is a transform pattern to apply to each input line to extract the taget file or directory path.  It can consist of any combination of capturing group that is identified by a number or a name.
* --cs: Specifies a case-sensitive matching. It's an error to specify this option without the '--rx' switch.
* --pi: For each result, prepends its input line, using 'sep' as separator. If 'sep' is missing, a colon is used.
* --h: Displays this help and exits.

# remove
Returns for every input line a new string in which all occurrences of a specified substring are deleted.
Type: Mutator.
Usage: remove str [--cs][--pi [sep]][--h]
* str: The non-empty substring to remove all occurrences.
* --cs: Specifies case-sensitive matching.
* --pi: For each result, prepends its input line, using 'sep' as separator. If 'sep' is missing, a colon is used.
* --h: Displays this help and exits.

# renFiles
Rename or move each input file, then returns the processed files.
Type: Action.
Usage: renFiles ptrn dest [src] [--ri][--ovw][--cs][--h]
* ptrn: Represents a regular expression pattern to match. Input files for which ‘ptrn’ does not match are ignored.
* dest: A transform pattern to apply to each input line to extract the new path or name for the input file. It can consist of any combination of literal text and substitutions, such as capturing group that is identified by a number or a name.
* src: A transform pattern to apply to each input line to extract the source file. It can consist of any combination of literal text and substitutions, such as capturing group that is identified by a number or a name. If this parameter is missing, the input string is considered to be the source file.
* --ri: Returns the processed input files. If this flag is omitted, the command returns the destination files.
* --ovw: Overwrites the destination file if it already exists.
* --cs: Specifies case-sensitive matching.
* --h: Displays this help and exits.

# reorder
Sorts the input lines in ascending order. This command performs a stable sort.
Type Mutator.
Usage: reorder [--rx ptrn key][--cs][--dsc][--h]
* --rx: Use regular expression to distinguish input lines. 'ptrn' represents a regular expression pattern to match.  The input lines for which ‘ptrn’ does not match are ignored. 'key' is a transform pattern to apply to each input line to extract the key used to distinguish input lines. It can consist of any combination of capturing group that is identified by a number or a name. 
* --cs: Specifies case-sensitive comparison and/or pattern matching.
* --dsc: Sorts the input in descending order.
* --h: Displays this help and exits.

# repeat
Generates a sequence that contains one repeated string.
Type: Generator.
Usage: repeat str count [--h]
* str: The string to be repeated.
* count: The number of times to repeat the string in the generated sequence.
* --h: Displays this help and exits.

# reverse
Inverts the order of the input. The output is a sequence whose elements correspond to those of the input sequence in reverse order.
Type: Mutator.
Usage: reverse [--h]
* --h: Displays this help and exits.

# serialize
 Inserts incremental numeric series of digits at the beginning of every input string.
 Type: Mutator.
Usage: serialize [sep] [--init start][--stp step][--h]
* sep: The string to use as a separator. If ‘sep’ is missing, an colon is used as separator.
* --init: Specifies the starting number. The default is 0.
* --stp: Specifies an optional increment. The default is 1. 'step', the increment, can be any non-zero integer.
* --h: Displays this help and exits.

# shuffle
Rearranges the order of the input randomly.
Type: Mutator.
Usage: shuffle [--h]
* --h: Displays this help and exits.

# skip
Bypasses input strings that match the specified pattern and then returns the remaining lines.
Type: Filter.
Usage: skip ptrn [--cs][--h]
* ptrn: The regular expression pattern to match.
* --cs: Specifies case-sensitive matching.
* --h: Displays this help and exits.

# skipFirst
Bypasses a specified number of input lines and then returns the remaining lines.
Type: Filter.
Usage: skipFirst [count] [--h]
* count: The number of lines to skip before returning the remaining lines. The default is 1.
* --h: Displays this help and exits.

# skipLast
Returns the input lines with the last 'count' lines omitted.
Type: Filter.
Usage: skipLast [count] [---h]
* count: The number of lines to omit from the end of the input. The default is 1.
* --h: Displays this help and exits.

# skipUntil
Bypasses lines from input until a specified pattern is matched, and then returns the remaining lines.
Type: Filter.
Usage: skipUntil ptrn [--cs][--h]
* ptrn: A regular expression pattern to match.
* --cs: Specifies case-sensitive matching.
* --h: Displays this help and exits.

# skipWhile
Bypasses lines from input as long as a specified pattern is matched, and then returns the remaining lines.
Type: Filter.
Usage: skipWhile ptrn [--cs][--h]
* ptrn: A regular expression pattern to match.
* --cs: Specifies case-sensitive matching.
* --h: Displays this help and exits.

# slice
Retrieves a substring from each input line. The substring starts at a specified character position and continues to the end of the line or has a specified length.
Type: Mutator.
Usage: slice ndxStart [count][--pi [sep]][--h]
* ndxStart: The zero-based starting character position of a substring in the input line. The input line is skiped if 'ndxStart' is greater than or equal to the length of the line.
* count: The number of characters in the substring. If this parameter is missing, the substring starts at 'ndxStart' character position and continues to the end of the line. The input line is skiped if 'ndxStart' + 'count' is greater than the length of the input string.
* --pi: For each result, prepends its input line, using 'sep' as separator. If 'sep' is missing, a colon is used.
* --h: Displays this help and exits.

# soak
Soaks up all input before returning it.
Type: Action
Usage: soak [--h]
* --h:  Display this help and exits.

# split
Splits each input line into substrings that are based on the provided separator. Each substring is retuned as a line.
Type: Mutator.
Usage: split [sep][--nopty][--trim][--h]
* sep: A string that delimits the substrings in the input lines. If ‘sep’ is absent, white-space characters are assumed to be the delimiters.
* --nopty: Omit empty substrings from the result.
* --trim: Trim white-space characters from each substring in the result.
* --h: Displays this help and exits.

# startsWith
Returns all input lines that starts with a specified string.
Type: Filter.
Usage: startsWith prefix [--cs][--h]
* prefix: The string to compare to the sub-string at the beginning of each input line.
* --cs: Specifies case-sensitive matching.
* --h: Display this help and exits.

# substitute
Returns, for each input line, a new string in which all occurrences of a specified string in the current line are replaced with another specified string.
Type: Mutator.
Usage: substitute oldStr newStr [--cs][--pi [sep]][--h]
* oldStr: The substring to be replaced.
* newStr: The string to replace all occurrences of 'oldStr'.
* --cs: Specifies case-sensitive matching.
* --pi: For each result, prepends its input line, using 'sep' as separator. If 'sep' is missing, a colon is used.
* --h: Display this help and exits.

# take
Filters input lines based on a regular expression pattern.
Type: Filter.
Usage: take ptrn [--cs][--h]
* ptrn: The regular expression pattern to match.
* --cs: Specifies case-sensitive matching.
* --h: Displays this help and exits.

# takeFirst
Returns a specified number of input lines and then skips the remaining ones.
Type: Filter.
Usage: takeFirst [count][--h]
* count: The number of input lines to return. The default is 1.
* --h: Displays this help and exits.

# takeLast
Returns the last 'count' input lines.
Type: Filter.
Usage: takeLast [count][--h]
* count: The number of lines to take from the end of the input sequence. The default is 1.
* --h: Displays this help and exits.

# takeUntil
Returns lines from the input sequence until a specified pattern is matched, and then skips the remaining ones.
Type: Filter.
Usage: takeUntil ptrn [--cs][--h]
* ptrn: The regular expression pattern to match.
* --cs: Specifies case-sensitive matching.
* --h: Displays this help and exits.

# takeWhile
Returns lines from the input sequence as long as a specified pattern is matched, and then skips the remaining ones.
Type: Filter
Usage: takeWhile ptrn [--cs][--h]
* ptrn: The regular expression pattern to match.
* --cs: Specifies case-sensitive matching.
* --h: Display this help and exits.

# throttle (MS Windows Only)
Displays a maximum of 'N' lines at a time and then waits the user presses a key to display the next 'N' lines. The 'Q' and 'ESC' keys are used to interrupt the program execution.
Type: Consumer.
Usage: throttle [N] [--h]
* N: The number of lines to display each time. The default is 1.
* --h: Displays this help and exits.

# toLowerCase
Returns the input lines converted to lowercase. This command uses the casing rules of the current culture to convert each character in the input to its lowercase equivalent. If a character does not have a lowercase equivalent, it is included unchanged in the returned string.
Type: Mutator
Usage: toLowerCase [--h]
* --h: Displays this help and exits.

# toUpperCase
Returns the input string converted to uppercase. This command uses the casing rules of the current culture to convert each character in the input to its uppercase equivalent. If a character does not have a uppercase equivalent, it is included unchanged in the returned string.
Type: Mutator.
Usage: toUpperCase [--h]
* --h: Display this help and exits.

# wrap
For each input line, puts a line feed every 'count' characters if it does not reach a new line before that point.
Type: Mutator.
Usage: wrap count [--h]
* count: A positive interger which correspond to the maximum length of a line.
* --h: Displays this help and exits.

# TODO
## checksum
Generates and returns a checksum value for every input line.
Type: Mutator.
Usage: checksum [--rx pattern target [--cs]][--f][--pin [sep]][--h]
* --rx: Use regular expression to extract, from the input line, the target string.  ‘pattern’ represents a regular expression pattern to match against each input line. The input lines for which ‘pattern’ is not matched are ignored. ‘target’ is the taget string.  It can consist of any combination of capturing group that is identified by a number or a name.
* --cs: Specifies a case-sensitive matching. It's an error to specify this option without the '--rx' switch.
* --f: The inout string represents a file path for whitch to caclulate the checksum.
* --pin: For each result, prepend its input line, using 'sep' as separator. If 'sep' is missing, a colon is used.
* --h: Display this help and exits.

## expand
For each input line, converts tab characters into groups of space characters and Vice-versa.
Type: Mutator.
Usage: expand count [--spc][--f [--rx patten src dest [--cs]]][--h]
* count: Number of space equivqlent to one tab.
* --spc: Converts white-spaces to tabs instead of the other way around.
* --f: Each input line represents a file path. If this switch is present, the command process each input file and return the result.
* --rx: Use regular expression to extract, from the input line, the source and destionation files path.  ‘pattern’ represents a regular expression pattern to match against each input line. The input lines for which ‘pattern’ is not matched are ignored. ‘src’ is the file path to process.  'dest' is the file path where to save the processing. Both can consist of any combination of capturing group that is identified by a number or a name. To use this swicth, '--f' must be present.
* --cs: Specifies a case-sensitive matching. It's an error to specify this option without the '--rx' switch.
* --h:  Display this help and exits.

## baseName
For every input path,  deletes any prefix up to the last directory separator character and 
returns the result.
Type: Mutator.
Usage: baseName [--rx ptrn path [--cs]][--pin [sep]][--h]
* --rx: Use regular expression to extract, from the input line, the target path. ‘ptrn’ represents a regular expression pattern to match against each input line. The input lines for which ‘ptrn’ does not are ignored. ‘path’ is a transform pattern to apply to each input line to extract the target  path. It can consist of any combination of literal text and substitutions based on 'ptrn', such as capturing group that is identified by a number or a name.
* --pin: For each result, prepend its input string, using 'sep' as separator. If 'sep' is missing, a colon is used.
* --cs: Specifies a case-sensitive matching. It's an error to specify this option without the '--rx' switch.
* --h: Displays this help and exits.

## toProperCase
For every input string, the first character of every word is made uppercase, all others made lowercase.
Type: Mutator.
Usage: toProperCase [--h]
* --h: Display this help and exits.

## toSentenceCase
The first character of each input string is made uppercase, all others made lowercase.
Type: Mutator.
Usage: toSentenceCase [--h]
* --h: Display this help and exits.