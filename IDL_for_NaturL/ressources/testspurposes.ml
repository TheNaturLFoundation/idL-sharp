while true do
read_line();
print_endline {|{ "jsonrpc": "2.0", "id": 1, "params": [{"label": "x"},{"label": "y"}]}|}
(**print_endline {|{ "jsonrpc": "2.0", "id": 1, "result": { "uri": "file:///Users/Adrian/Desktop/test.ntl", "range": {"start": {"line": 0,"character": 4},"end": {"line": 0,"character": 11}}}}|}**)
done


(**print_endline {|{ "jsonrpc": "2.0", "id": 1, "params": [{"label": "x"},{"label": "y"}]}|}**)