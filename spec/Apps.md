# about
provide help information for Link package commands.

Usage: about [appName] [--h]
* appName: Command to display information about. If this argument is not provided, all the commands available in the package are listed.
* --h: Display this help and exits.

# append
adds a string to the end of each input line.

Usage: append suffix [--h]
* suffix: The string to append to each input line.
* --h: Display this help and exits.

# count
counts the number of input lines.

Usage: count [--h]
* --h: Display this help and exits.

# countFreq
returns the number of occurrences of each character or word that appears in the input text.

Usage: countFreq [--h][--wrd][--frq [pcs]][--sep separator]
* --wrd: calculate words frequency instead of characters.
* --frq: display the  frequency for each item. 'pcs' is the precision specifier, the number of fractional digits to display for the frequency value. The default is to display 4 digits after the decimal point.
* --sep: uses 'separator' as a string separator between the item and its count. If 'separator' is missing, a colon is used.
* --h: display this help and exits.

# copyFile
copy the input files to another location. Any and all directories specified in 'dest' are created, unless they already exist. The command return the copied files.

Usage: copyFile pattern dest [src] [--rin][--ovw][--cs][--h]
* pattern:Represents a regular expression pattern to match. The input lines for which 'pattern' is not matched are ignored.
* dest:A transform pattern to apply to each input line. It can consist of any combination of literal text and substitutions based on 'pattern', such as capturing group that is identified by a number or a name.
* src: An optional transform pattern to apply to each input line to distinguish the source file. It can consist of any combination of literal text and substitutions based on 'pattern', such as capturing group that is identified by a number or a name. If 'src' is missing, the input line is considered the source file.
* --rin: Return the copied input files instead of destination files.
* --ovw: Destination file should be replaced if it already exists.
* --cs: Specifies case-sensitive matching.
* --h: Displays this help and exits.

# copyToDir
copies existing input files to a different directory.

Usage: copyToDir dest [--rin][--ovw][--h]
* dest: The name of the directory to copy to. Any and all directories specified in 'dest' are created, unless they already exist.
* --rin: Return the copied input files instead of destination files.
* --ovw: Destination file should be replaced if it already exists.
* --h: Displays this help and exits.

# createDir
Usage: createDir [--rx pattern dir][rin][--h]
# dateRange
Usage: dateRange count [--min date][--stp days][--h]
# delDir
Usage: delDir [--ety][--rin][--h]
# delFile
Usage: delFile [--rx pattern file][--rin][--h]
# dirSize
Usage: dirSize [--rx pattern dir][--pin [sep]][--fmt][--h]
# distinct
Usage: distinct [--rx patten key][--cs][--h]
# echoFile 
Usage: echoFile [--rx pattern file][--h]
# endsWith
Usage: endsWith str [--cs][--h]
# fileDate
Usage: fileDate [--crd][--rx pattern file][--pin [sep]][--h]
# fileDir
Usage: fileDir [--rx pattern path][--pin [sep]][--h]
# fileExt
Usage: fileExt [--rx pattern path][--pin [sep]][--h]
# fileName
Usage: fileName [--rx pattern path][--pin [sep]][--h]
# fileSize
Usage: fileSize [--rx pattern file][--pin [sep]][--fmt][--h]
# groupBy
Usage: groupBy pattern key [--cs][--h]
# indexOf
returns the zero-based index of the first occurrence of a specified string within each input line. 

Usage: indexOf str [--cs][--pin [sep]][--h]

# insert
returns, for each input line, a new string in which a specified string is inserted at a specified index.

Usage: insert str ndx [--pin [sep]][--h]
# intRange
Usage: intRange count [--min n][--stp step][--h]
# invert
Usage: invert [--h]
# invertCase
Usage: invertCase [--h]

# length
returns the number of characters for each input string.

Usage: length [--pin [sep]][--h]
* --pin: prepend for each result its input line separated by the string 'sep'. If 'sep' is missing, a colon is used as separator.
* --h: display this help and exits.

# listDir
Usage: listDir [--r [depth]][--hid][--dp][--h]
# listFiles
Usage: listFile [--r [depth]][--hid][--dp][--h]
# merge
Usage: join [sep] [--n count][--h]
# moveToDir
Usage: moveToDir dir [--rin][--ovw][--h]
# padEnd
Usage: padEnd len [--str suffix][--h]
# padStart
Usage: padStart len [--str suffix][--h]
# prepend
Usage: prepend prefix [--h]
# randomizeCase
Usage: randomizeCase [--h]
# remove
Usage: remove str [--cs][--pin [sep]][--h]
# renameFile
Usage: renameFile pattern name [src] [--rin][--ovw][--cs][--h]
# reorder
Usage: reorder [--rx patten key][--cs][--dsc][--h]
# repeat
Usage: repeat str count [--h]
# reverse
Usage: reverse [--rx pattern strToReverse][--pin [sep]][--h]
# select
Usage: select pattern output [--cs] \[--h]
# serialize
Usage: serialize [--n start][--stp step][--fmt][--h]
# shuffle
Usage: shuffle [--h]
# skip
Usage: skip pattern [--cs][--h]
# skipFirst
Usage: skipFirst [count] [--h]
# skipLast
Usage: skipLast [count] [---h]
# skipUntil
Usage: skipUntil pattern [--cs][--h]
# skipWhile
Usage: skipWhile pattern [--cs][--h]
# slice
Usage: slice ndxStart [count][--h]
# split
Usage: split [sep][--oes][--tws][--h]
# startsWith
Usage: startsWith prefix [--cs][--h]
# substitute
Usage: substitute oldStr [newStr] [--cs][--pin [sep]][--h]
# take
Usage: take pattern [--cs][--h]
# takeFirst
Usage: takeFirst [count][--h]
# takeLast
Usage: takeLast [count][--h]
# takeUntil
Usage: takeUntil pattern [--cs][--h]
# takeWhile
Usage: takeWhile pattern [--cs][--h]
# throttle
Usage: throttle [nLines][--h]
# toLowerCase
Usage: toLowerCase [--h]
# toProperCase
Usage: toUpperCase [--h]
# toSentenceCase
Usage: toSentenceCase [--h]
# toUpperCase
Usage: UpperCase [--h]
