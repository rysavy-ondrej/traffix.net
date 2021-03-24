meta:
  id: http_response
  file-extension: http_response
seq:
  - id: response_line 
    type: http_response_line
  - id: headers
    type: http_header_lines
  - id: body
    type: str
    size-eos: true
    encoding: ASCII 
types:
  http_response_line:
    seq:
    - id: version
      type: str
      encoding: ASCII
      terminator: 0x20
    - id: status_code
      type: str
      encoding: ASCII
      terminator: 0x20
    - id: status_message
      type: str
      encoding: ASCII
      terminator: 0xa
  http_header_lines:
    seq:
    - id: header_line
      type: str
      terminator: 10
      eos-error: false
      encoding: ascii
      repeat: until
      repeat-until: _ == "\r\n"