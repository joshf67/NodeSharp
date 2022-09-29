parser grammar NodeSharpParser;

options { tokenVocab=NodeSharpLexer; }

program: line* EOF;
 
line: statement | ifBlock | whileBlock;
block: '{' line* '}';

statement: ( internal_functionCall | assignment | functionCall) ';';

ifBlock: IF '(' expression ')' WHITESPACES*  block WHITESPACES*  (ELSE WHITESPACES*  elseIfBlock);
elseIfBlock: block | ifBlock;
whileBlock: 'while' WHITESPACES* '(' expression ')' WHITESPACES*  block;

assignment: (SCOPE)? IDENTIFIER ASSIGNMENT WHITESPACES* expression;
functionCall: (SCOPE)? IDENTIFIER '(' expression_list? ')';
function_creation: (SCOPE)? 'function' WHITESPACES IDENTIFIER '(' expression_list ')' block;

indexLookup: (SCOPE)? IDENTIFIER '[' expression ']';

array: '[' expression_list ']';

getProperty: (SCOPE)? IDENTIFIER '.' IDENTIFIER;

expression_list
	: expression (WHITESPACES* ',' WHITESPACES* expression)*
	;

expression
    : getProperty                                                               #getPropertyExpression
    | constant                                                                  #constantExpression
    | (SCOPE)? IDENTIFIER                                                       #identifierExpression
    | '(' expression ')'                                                        #parenthesizedExpression
    | '!' expression                                                            #notExpression
    | (SCOPE)? IDENTIFIER arithmeticOp expression                               #arithmeticExpression
    | (SCOPE)? affixOp IDENTIFIER                                               #prefixExpression
    | (SCOPE)? IDENTIFIER affixOp                                               #suffixExpression
    | expression multiplyOp expression                                          #multiplicativeExpression
    | expression additionOp expression                                          #additiveExpression
    | expression comparisonOp expression                                        #comparisonExpression
    | function_creation                                                         #functionCreationExpression
    | indexLookup                                                               #indexerExpression
    | array                                                                     #arrayExpression
    ;

internal_functionCall
    : trigonometryOp '(' expression ')'                                         #trigonometryExpression
    | 'Arctan2' '(' expression WHITESPACES* ',' WHITESPACES* expression ')'     #trigonometryArcTan2Expression
    ;

constant: BOOL | VECTOR3 | NUMBER | STRING | AREA_MONITOR | EQUIPMENT_TYPE | GRENADE_TYPE;
multiplyOp: '*' | '/' | '%' | '^'| '^^';
additionOp: '+' | '-';
arithmeticOp: '+=' | '-=' | '*=' | '/=' | '^=' | '^^=';
affixOp: '++' | '--';
trigonometryOp: 'Sin' | 'Cos' | 'Tan' | 'Arcsin' | 'Arccos';
comparisonOp: '==' | '!=' | '>' | '<' | '>=' | '<=';