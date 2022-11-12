lexer grammar NodeSharpLexer;

channels { COMMENTS_CHANNEL }

IF:                     'if';
ELSE:                   'else';
WHILE:                  'while';
FUNCTION:               'function';
CONTINUE:               'continue';

SIN:                    'Sin';
COS:                    'Cos';
TAN:                    'Tan';
ARCSIN:                 'Arcsin';
ARCCOS:                 'Arccos';
ARCTAN2:                'Arctan2';

OPEN_BRACE:             '{';
CLOSE_BRACE:            '}';
OPEN_BRACKET:           '[';
CLOSE_BRACKET:          ']';
OPEN_PARENS:            '(';
CLOSE_PARENS:           ')';
DOT:                    '.';
COMMA:                  ',';
SEMICOLON:              ';';


//Operators
PLUS:                   '+';
MINUS:                  '-';
STAR:                   '*';
DIV:                    '/';
PERCENT:                '%';
POWER:                  '^';
SQRT:                   '^^';

OP_INC:                 '++';
OP_DEC:                 '--';

BANG:                   '!';
OP_EQ:                  '==';
OP_NE:                  '!=';
OP_L:                   '<';
OP_G:                   '>';
OP_LE:                  '<=';
OP_GE:                  '>=';

ASSIGNMENT:             '=';
OP_ADD_ASSIGNMENT:      '+=';
OP_SUB_ASSIGNMENT:      '-=';
OP_MULT_ASSIGNMENT:     '*=';
OP_DIV_ASSIGNMENT:      '/=';
OP_MOD_ASSIGNMENT:      '%=';
OP_POW_ASSIGNMENT:      '^=';
OP_SQRT_ASSIGNMENT:     '^^=';

COMMENT_LINE:           '//';
COMMENT_LINE_DOC:       '///';
COMMENT_BLOCK_START:    '/*';
COMMENT_BLOCK_END:      '*/';
COMMENT:                (((COMMENT_LINE | COMMENT_LINE_DOC) (.)*? (NewLine | EOF))
                        | (COMMENT_BLOCK_START (.)*? (COMMENT_BLOCK_END | EOF))) -> channel(COMMENTS_CHANNEL);

WHITESPACES:            (Whitespace | NewLine)+ -> channel(HIDDEN);

fragment NewLine
	: '\r\n' | '\r' | '\n'
	| '\u0085' // <Next Line CHARACTER (U+0085)>'
	| '\u2028' //'<Line Separator CHARACTER (U+2028)>'
	| '\u2029' //'<Paragraph Separator CHARACTER (U+2029)>'
	;

fragment Whitespace
	: UnicodeClassZS //'<Any Character With Unicode Class Zs>'
	| '\u0009' //'<Horizontal Tab Character (U+0009)>'
	| '\u000B' //'<Vertical Tab Character (U+000B)>'
	| '\u000C' //'<Form Feed Character (U+000C)>'
	;
	
fragment UnicodeClassZS
	: '\u0020' // SPACE
	| '\u00A0' // NO_BREAK SPACE
	| '\u1680' // OGHAM SPACE MARK
	| '\u180E' // MONGOLIAN VOWEL SEPARATOR
	| '\u2000' // EN QUAD
	| '\u2001' // EM QUAD
	| '\u2002' // EN SPACE
	| '\u2003' // EM SPACE
	| '\u2004' // THREE_PER_EM SPACE
	| '\u2005' // FOUR_PER_EM SPACE
	| '\u2006' // SIX_PER_EM SPACE
	| '\u2008' // PUNCTUATION SPACE
	| '\u2009' // THIN SPACE
	| '\u200A' // HAIR SPACE
	| '\u202F' // NARROW NO_BREAK SPACE
	| '\u3000' // IDEOGRAPHIC SPACE
	| '\u205F' // MEDIUM MATHEMATICAL SPACE
	;
	
//Variable Types

VECTOR3: 'Vector3(' WHITESPACES* NUMBER WHITESPACES* ',' WHITESPACES* NUMBER WHITESPACES* ',' WHITESPACES* NUMBER WHITESPACES* ')';
NUMBER: [0-9]+ | ([0-9.]+ '.' [0-9]+);

STRING: 'String.' IDENTIFIER;
BOOL: 'true' | 'false';

AREA_MONITOR: 'AreaMonitor';
EQUIPMENT_TYPE: 'EquipmentType';
GRENADE_TYPE: 'GrenadeType';

SCOPE: ('global ' | 'Global ' | 'local ' | 'Local ' | 'param ' | 'Param ') WHITESPACES*;
IDENTIFIER: [a-zA-Z_][a-zA-Z0-9_]*;