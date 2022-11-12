parser grammar NodeSharpParser;

options { tokenVocab=NodeSharpLexer; }


program: line* EOF;
 
line: statement | branchBlock | whileBlock | functionCreation;
block: '{' line* '}';

statement: (internal_functionCall | assignment | functionCall) ';';

conditionalBlock: '(' expression ')' block;
continueBlock: CONTINUE ';';
branchBlock: IF conditionalBlock ((ELSE branchBlock) | (ELSE block))?  continueBlock?;
whileBlock: 'while' conditionalBlock;

assignment: (SCOPE)? IDENTIFIER ASSIGNMENT assignmentParameters;
assignmentParameters: expression | functionCall;

functionCall: (SCOPE)? IDENTIFIER '(' functionParameters? ')';
functionParameters: (expression | functionCall) (',' (expression | functionCall))*;

functionCreation: (SCOPE)? 'function' IDENTIFIER '(' functionCreationParameters* ')' block;
functionCreationParameters: IDENTIFIER (',' IDENTIFIER)*;

indexLookup: (SCOPE)? IDENTIFIER '[' expression ']';

array: '[' arrayParameters ']';
arrayParameters: expression (',' expression)*;

getProperty: (SCOPE)? IDENTIFIER '.' IDENTIFIER;

expression
    : getProperty                                                               #getPropertyExpression
    | constant                                                                  #constantExpression
    | (SCOPE)? IDENTIFIER                                                       #identifierExpression
    | '(' expression ')'                                                        #parenthesizedExpression
    | '!' expression                                                            #notExpression
    | (SCOPE)? IDENTIFIER arithmeticOp expression                               #arithmeticExpression
    | (SCOPE)? IDENTIFIER '^^='                                                 #squareRootAssignmentExpression
    | (SCOPE)? affixOp IDENTIFIER                                               #prefixExpression
    | (SCOPE)? IDENTIFIER affixOp                                               #suffixExpression
    | expression multiplyOp expression                                          #multiplicativeExpression
    | expression '^^'                                                           #squareRootExpression
    | expression additionOp expression                                          #additiveExpression
    | expression comparisonOp expression                                        #comparisonExpression
    | indexLookup                                                               #indexerExpression
    | array                                                                     #arrayExpression
    ;

internal_functionCall
    : trigonometryOp '(' expression ')'                                         #trigonometryExpression
    | 'Arctan2' '(' expression WHITESPACES* ',' WHITESPACES* expression ')'     #trigonometryArcTan2Expression
    ;

constant: BOOL | VECTOR3 | NUMBER | STRING | AREA_MONITOR | EQUIPMENT_TYPE | GRENADE_TYPE;
multiplyOp: '*' | '/' | '%' | '^';
additionOp: '+' | '-';
arithmeticOp: '+=' | '-=' | '*=' | '/=' | '^=' ;
affixOp: '++' | '--';
trigonometryOp: 'Sin' | 'Cos' | 'Tan' | 'Arcsin' | 'Arccos';
comparisonOp: '==' | '!=' | '>' | '<' | '>=' | '<=';