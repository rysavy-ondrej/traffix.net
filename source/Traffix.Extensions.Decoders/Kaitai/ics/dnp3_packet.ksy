meta:
  id: dnp3_packet
  license: CC0-1.0
  ks-version: 0.7
  endian: be
doc: |
  A simplified DNP3 parser. It only extracts information up to the application function code 
  which needs to be located wihtin the first data chunk. It ignores objects and does not
  processes other than the first fragment/chunk. 
seq:
  - id: frame_header
    type: dnp3_frame_header
  - id: first_chunk
    type: dnp3_first_chunk
    size: "(_io.pos + 18 <= _io.size) ? 18 : (_io.size - _io.pos)" 
    if:  transport_control & 0x40 == 0x40   # is it the first fragment?
  - id: next_chunks 
    type: dnp3_chunk
    repeat: eos
instances:
  transport_control:   
    pos: 10           
    type: u1 
    doc: |
      Provides transport control byte. We need to determine transport control 
      in advance to decide if we have the first chunk or not.
types:
  dnp3_chunk:
    seq:
    - id: data
      size: "(_io.pos + 18 <= _io.size) ? 16 : (_io.size - _io.pos) - 2"
    - id: checksum
      type: u2
    doc: |
      Represents a DNP3 chunk. The chunk always consists of up to 16 data bytes
      and checksum value.  
  dnp3_first_chunk:
    seq:
    - id: transport_control
      type: dnp3_transport_control
    - id: application
      type: dnp3_application 
    - id: data
      size: _io.size - (_io.pos + 2)
    - id: checksum
      type: u2
    doc: |
      Represents a DNP3 chunk. The chunk always consists of up to 16 data bytes
      and checksum value.
  dnp3_frame_header: 
    seq:
      - id: sync
        contents: [0x05, 0x64]
      - id: length
        type: u1
      - id: control
        type: dnp3_frame_header_control
      - id: destination
        type: u2
      - id: source
        type: u2
      - id: checksum
        type: u2
    doc: |
      Represents DNP3 link layer header.
  dnp3_application:
    seq:
      - id: application_control
        type: dnp3_application_control
      - id: function_code
        type: u1
        enum: dnp3_function_code
      - id: internal_indication
        type: dnp3_internal_indications
        if: function_code == dnp3_function_code::dnp3_response or function_code == dnp3_function_code::dnp3_unsolicited_response
    doc: |
      Contains application control and mainly function code of DNP3 message.
  dnp3_frame_header_control:
    seq:
      - id: direction
        type: b1
        enum: dnp3_direction 
      - id: primary
        type: b1
        enum: dnp3_primary 
      - id: frame_count_bit
        type: b1
      - id: frame_count_valie
        type: b1
      - id: control_function_code
        type: b4
  dnp3_application_control:
    seq:
      - id: fir
        type: b1
      - id: fin
        type: b1
      - id: con
        type: b1
      - id: uns
        type: b1
      - id: seq
        type: b4
  dnp3_transport_control:
    seq:
      - id: final
        type: b1
      - id: first
        type: b1
      - id: sequence
        type: b6 
  dnp3_internal_indications:
    seq:
      - id: device_restart
        type: b1
      - id: device_trouble
        type: b1
      - id: local_control
        type: b1
      - id: need_time
        type: b1
      - id: class_3_event
        type: b1
      - id: class_2_event
        type: b1
      - id: class_1_event
        type: b1
      - id: all_stations
        type: b1
      - id: reserved
        type: b2
      - id: configuration_corrupt
        type: b1
      - id: already_executing
        type: b1
      - id: event_buffer_overflow
        type: b1
      - id: parameter_error
        type: b1
      - id: object_unknown
        type: b1
      - id: function_not_supported
        type: b1
enums:
  dnp3_direction:
    0x00: from_outstation
    0x01: from_master
  dnp3_primary:
    0x00: sec_to_pri
    0x01: pri_to_sec
  dnp3_function_code:
    0x00: dnp3_confirm
    0x01: dnp3_read
    0x02: dnp3_write
    0x03: dnp3_select
    0x04: dnp3_operate
    0x05: dnp3_dir_operate
    0x06: dnp3_dir_operate_no_resp 
    0x07: dnp3_freeze 
    0x08: dnp3_freeze_no_resp 
    0x09: dnp3_freeze_clear 
    0x0A: dnp3_freeze_clear_no_resp 
    0x0B: dnp3_freeze_at_time 
    0x0C: dnp3_freeze_at_time_no_resp 
    0x0D: dnp3_cold_restart 
    0x0E: dnp3_warm_restart 
    0x0F: dnp3_initialize_data
    0x10: dnp3_initialize_application
    0x11: dnp3_start_application
    0x12: dnp3_stop_application
    0x13: dnp3_save_configuration
    0x14: dnp3_enable_unsolicited
    0x15: dnp3_disable_unsolicited
    0x16: dnp3_assign_class
    0x17: dnp3_delay_measurement
    0x18: dnp3_record_current_time
    0x19: dnp3_open_file
    0x1A: dnp3_close_file
    0x1B: dnp3_delete_file
    0x1C: dnp3_get_file_information
    0x1D: dnp3_authenticate_file
    0x1E: dnp3_abort_file
    #Responses (Hex)
    0x81: dnp3_response
    0x82: dnp3_unsolicited_response 
      

       
  