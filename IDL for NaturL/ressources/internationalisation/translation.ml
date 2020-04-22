open Yojson

type language =
  | French
  | English

let lang = ref English

let setLang l =
  lang := l

let set_lang_of_string = function
  | "french" -> setLang French
  | "english" -> setLang English
  | _ -> failwith "Unknown language"

type key =
  | SyntaxError
  | TypeError
  | NameError
  | ImportError
  | NameTypeMessage
  | NameButGotMessage
  | HasTypeMessage
  | ButGotMessage
  | ReturnTypeMatchMessage
  | UnexpectedReturn
  | UnexpectedToken
  | UnexpectedEOF
  | ExpectedDebut
  | UnexpectedDebut
  | UnexpectedFin
  | ExpectedReturn
  | ExpectedFin
  | UnexpectedChar
  | InFunctionDefinition
  | BreakingReturn
  | AlwaysTrue
  | AlwaysFalse
  | MissingKeyword
  | UnknownVariable
  | InvalidFunctionDefinition
  | UnknownType
  | ExpectedOperand
  | InvalidExpression
  | InvalidTokenExpression
  | ReservedKeyword
  | TokenCapture
  | MissingClosingParenthesis
  | MissingClosingBracket
  | UnexpectedParenthesis
  | UnexpectedBracket
  | UnknownOperator
  | InvalidOperation
  | AndType
  | CannotCompare
  | VariablesOfType
  | NotCallable
  | UnknownFunction
  | TheType
  | NotSubscriptable
  | ListIndicesIntegers

let json = ref (Basic.from_file "internationalisation/translation.json")


let getLangID = function
  | French -> "fr"
  | English -> "en"

let get_member_from_JSON value =
  let high_member = Yojson.Basic.Util.member value !json in
  Yojson.Basic.Util.to_string (Yojson.Basic.Util.member (getLangID !lang) high_member)

let get_string key  = match key with
  | SyntaxError -> get_member_from_JSON "SyntaxError"
  | TypeError -> get_member_from_JSON "TypeError"
  | NameError -> get_member_from_JSON "NameError"
  | ImportError -> get_member_from_JSON "ImportError"
  | NameTypeMessage -> get_member_from_JSON "NameTypeMessage"
  | NameButGotMessage -> get_member_from_JSON "NameButGotMessage"
  | HasTypeMessage -> get_member_from_JSON "HasTypeMessage"
  | ButGotMessage -> get_member_from_JSON "ButGotMessage"
  | ReturnTypeMatchMessage -> get_member_from_JSON "ReturnTypeMatchMessage"
  | UnexpectedReturn -> get_member_from_JSON "UnexpectedReturn"
  | UnexpectedToken -> get_member_from_JSON "UnexpectedToken"
  | UnexpectedEOF -> get_member_from_JSON "UnexpectedEOF"
  | ExpectedDebut -> get_member_from_JSON "ExpectedDebut"
  | UnexpectedDebut -> get_member_from_JSON "UnexpectedDebut"
  | UnexpectedFin -> get_member_from_JSON "UnexpectedFin"
  | ExpectedReturn -> get_member_from_JSON "ExpectedReturn"
  | ExpectedFin -> get_member_from_JSON "ExpectedFin"
  | UnexpectedChar -> get_member_from_JSON "UnexpectedChar"
  | InFunctionDefinition -> get_member_from_JSON "InFunctionDefinition"
  | BreakingReturn -> get_member_from_JSON "BreakingReturn"
  | AlwaysTrue -> get_member_from_JSON "AlwaysTrue"
  | AlwaysFalse -> get_member_from_JSON "AlwaysFalse"
  | MissingKeyword -> get_member_from_JSON "MissingKeyword "
  | InvalidFunctionDefinition -> get_member_from_JSON "InvalidFunctionDefinition"
  | UnknownVariable -> get_member_from_JSON "InvalidVariable"
  | UnknownType -> get_member_from_JSON "UnknownType"
  | ExpectedOperand -> get_member_from_JSON "ExpectedOperand"
  | InvalidExpression -> get_member_from_JSON "InvalidExpression"
  | InvalidTokenExpression -> get_member_from_JSON "InvalidTokenExpression"
  | ReservedKeyword -> get_member_from_JSON "ReservedKeyword"
  | TokenCapture -> get_member_from_JSON "TokenCapture"
  | MissingClosingParenthesis -> get_member_from_JSON "MissingClosingParenthesis"
  | MissingClosingBracket -> get_member_from_JSON "MissingClosingBracket"
  | UnexpectedParenthesis -> get_member_from_JSON "UnexpectedParenthesis"
  | UnexpectedBracket -> get_member_from_JSON "UnexpectedBracket"
  | UnknownOperator -> get_member_from_JSON "UnknownOperator"
  | InvalidOperation -> get_member_from_JSON "InvalidOperation"
  | AndType -> get_member_from_JSON "AndType"
  | CannotCompare -> get_member_from_JSON "CannotCompare"
  | VariablesOfType -> get_member_from_JSON "VariablesOfType"
  | NotCallable -> get_member_from_JSON "NotCallable"
  | UnknownFunction -> get_member_from_JSON "UnknownFunction"
  | TheType -> get_member_from_JSON "TheType"
  | NotSubscriptable -> get_member_from_JSON "NotSubscriptable"
  | ListIndicesIntegers -> get_member_from_JSON "ListIndicesIntegers"
