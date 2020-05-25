open Yojson

exception NoNaturLPath

let () =
  Printexc.register_printer
    (function
      | NoNaturLPath -> Some (Printf.sprintf ": no NATURLPATH environment variable is set on this computer")
      | _ -> None (* for other exceptions *)
    )

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

let to_int = function
  | SyntaxError -> 1
  | TypeError -> 2
  | NameError -> 3
  | ImportError -> 4
  | NameTypeMessage -> 5
  | NameButGotMessage -> 6
  | HasTypeMessage -> 7
  | ButGotMessage -> 8
  | ReturnTypeMatchMessage -> 9
  | UnexpectedReturn -> 10
  | UnexpectedToken -> 11
  | UnexpectedEOF -> 12
  | ExpectedDebut -> 13
  | UnexpectedDebut -> 14
  | UnexpectedFin -> 15
  | ExpectedReturn-> 16
  | ExpectedFin -> 17
  | UnexpectedChar -> 18
  | InFunctionDefinition -> 19
  | BreakingReturn -> 20
  | AlwaysTrue -> 21
  | AlwaysFalse -> 22
  | MissingKeyword -> 23
  | UnknownVariable -> 24
  | InvalidFunctionDefinition ->25
  | UnknownType -> 26
  | ExpectedOperand -> 27
  | InvalidExpression -> 28
  | InvalidTokenExpression -> 29
  | ReservedKeyword -> 30
  | TokenCapture -> 31
  | MissingClosingParenthesis -> 32
  | MissingClosingBracket -> 33
  | UnexpectedParenthesis -> 34
  | UnexpectedBracket -> 35
  | UnknownOperator -> 36
  | InvalidOperation -> 37
  | AndType -> 38
  | CannotCompare -> 39
  | VariablesOfType -> 40
  | NotCallable -> 41
  | UnknownFunction -> 42
  | TheType -> 43
  | NotSubscriptable -> 44
  | ListIndicesIntegers -> 45

let json =let path =
  try [Sys.getenv "NATURLPATH"; "internationalisation"; "translation.json"]
                      |> List.fold_left (fun s -> fun elt -> Filename.concat s elt) ""
  with Not_found -> raise NoNaturLPath
  in
    ref (Basic.from_file path)


let getLangID = function
  | French -> "fr"
  | English -> "en"

let get_member_from_JSON value =
  try
    let high_member = Yojson.Basic.Util.member value !json in
    Yojson.Basic.Util.to_string (Yojson.Basic.Util.member (getLangID !lang) high_member)
  with Yojson.Basic.Util.Type_error _ -> failwith ("JSON error with key " ^ value)

let get_string key  =
   (*print_int(to_int key) ;*)
   match key with
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
  | MissingKeyword -> get_member_from_JSON "MissingKeyword"
  | InvalidFunctionDefinition -> get_member_from_JSON "InvalidFunctionDefinition"
  | UnknownVariable -> get_member_from_JSON "UnknownVariable"
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
