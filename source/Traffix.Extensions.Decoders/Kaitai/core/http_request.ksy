meta:
  id: http_request
  file-extension: http_request
seq:
  - id: request_line
    type: http_request_line
  - id: headers
    type: http_header_lines
  - id: body
    type: str
    size-eos: true
    encoding: ASCII

types:
  http_request_line:
    seq:
    - id: command
      type: str
      encoding: ASCII
      terminator: 0x20
    - id: uri
      type: str
      encoding: ASCII
      terminator: 0x20
    - id: version
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
      
    