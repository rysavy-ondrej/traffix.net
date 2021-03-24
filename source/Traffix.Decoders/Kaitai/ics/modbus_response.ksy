meta:
  id: modbus_response_packet
  title: Modbus protocol parser for Query messages
  file-extension: raw
  endian: be
doc: |
  This is a simplified MODBUS parser. It only parses the header and identifies the function code of the message.
  A dedicated header is used on TCP/IP to identify the MODBUS Application Data Unit. It is called the MBAP header (MODBUS Application Protocol header).
  The problem with modbus messages is that there is not indication if the message is 
  Query or Response within the message. Parser for Query differs to that for Response. 
  We thus need to know whether the message is Query or Response before we start parsing.
  
doc-ref: 'http://www.modbus.org/docs/Modbus_Application_Protocol_V1_1b.pdf'
seq:
  - id: header
    type: mbap_header
  - id: status
    type: b1
    enum: modbus_status_code
  - id: function
    type: b7
    enum: modbus_function_code
  - id: data
    size: header.length - 2
types:
  mbap_header:
    seq:
    - id: transation_id
      type: u2
    - id: protocol_id
      type: u2
    - id: length
      type: u2
    - id: unit_identifier
      type: u1
      
enums:
  modbus_status_code:
    0: success
    1: error
  modbus_function_code:
    1: read_coil_status
    2: read_input_status
    3: read_holding_register
    4: read_input_registers
    5: write_single_coil
    6: write_single_register
    7: read_exception_status
    8: diagnostic
    11: get_com_event_counter
    12: get_com_event_log
    15: write_multiple_coils
    16: write_multiple_registers
    17: report_slave_id
    20: read_file_record
    21: write_file_record
    22: mask_write_register
    23: read_write_multiuple_registers
    24: read_fifo_queue
    43: read_device_identification