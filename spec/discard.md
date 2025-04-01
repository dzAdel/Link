# indexOf
Returns the zero-based index of the first occurrence of a specified string within each input line. Entries for which the string is not found are ignored.
Type: Mutator.
Usage: indexOf str [ndxStart] [--rx ptrn][--cs][--pin [sep]][--h]
* str: The string to seek.
* ndxStart: A non-negative integer, representing the search starting position. The default value is 0. Any input line length less than or equal to 'ndxStart' is ignored. 
* --rx: Use regular expression to distinguish between input lines. 'ptrn' represents a regular expression pattern to match. Input lines for which 'ptrn' does not match are ignored. If this option is present, 'str', the string to be searched for, can be formed by any combination of literal text and substitutions, such as capturing a group identified by a number or name.  If this switch is missing, 'str' is considered an ordinary string.
* --cs: Specifies a case-sensitive comparison/matching.
* --pin: For each result, prepend  its input line separated by the string 'sep'. If 'sep' is missing, a colon is used as separator.
* --h: Displays this help and exits.

# invert
Reverses each input line.
Type: Mutator.
Usage: invert [--rx ptrn substr][--pi [sep]][--h]
* --rx: Use regular expression to extract the target sub-string from the input lines. 'ptrn' represents a regular expression pattern to match. Input lines for which 'ptrn' does not match are ignored. If this option is present, 'substr', the sub-string to be invert, can be formed by any combination of literal text and substitutions, such as capturing a group identified by a number or name.  If this switch is missing, the whole input string is reversed.
* --h: Displays this help and exits.

# invertCase
For each input string, any character that were uppercase are changed to lowercase, and any that were lowercase are changed to uppercase.
Type: Mutator.
Usage: invertCase [--h]
* --h: Displays this help and exits.