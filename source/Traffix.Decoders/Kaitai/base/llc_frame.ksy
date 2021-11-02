meta:
  id: llc_frame
  title: LLC
  license: CC0-1.0
  endian: be
seq:
  - id: dsap_address
    type: b7
  - id: dsap_type
    type: b1
    enum: address_type_enum
  - id: ssap_address
    type:  b7
  - id: frame_type
    type: b1
    enum: frame_type_enum
  - id: control
    type: control_type
  - id: data
    size-eos: true
types:
  control_type:
    seq:
      - id: frame_content
        type:
          switch-on: type
          cases: 
            'control_frame_type::iframe': information
            'control_frame_type::sframe': supervisory
            'control_frame_type::uframe': unnumbered
    instances:
      control_byte:
        pos: 0
        type: u1
      type:
        value: 'control_byte & 0x1 == 0 ? 0 : (control_byte & 0x3)'
        enum: control_frame_type
  information:
    seq:
      - id: ts_sequence_number
        type: b7
      - id: type
        type: b1
      - id: tr_sequence_number
        type: b7
      - id: poll_final
        type: b1
  supervisory:
    seq: 
      - id: reserved
        type: b4
      - id: command
        type: b2
        enum: llc_sframe_command
      - id: type
        type: b2
      - id: tr_sequence_number
        type: b7
      - id: poll_final
        type: b1      
  unnumbered:
    seq:
      - id: mmm
        type: b3
      - id: poll_final
        type: b1
      - id: mm
        type: b2
      - id: type
        type: b2
enums:
  address_type_enum:
    0: individual_address
    1: group_address
  frame_type_enum:
    0: command_frame
    1: response_frame
  control_frame_type:
    0: iframe
    1: sframe
    2: uframe
  llc_sframe_command:
    0: receive_ready
    1: reject
    2: receive_not_ready
    3: unknown_command

  llc_address_table:
    0x00: null_lsap
    0x02: llc_sub_layer_management
    0x04: ibm_sna_path_control
    0x06: tcp_ipdod_internet_protocol
    0x08: ibm_sna_8
    0x0c: ibm_sna_c
    0x0e: proway_iec955_network_management_and_initialization
    0x10: netware
    0x14: iso_network_layer_oslan_1
    0x18: texas_instruments
    0x20: iso_network_layer_20
    0x32: dg_x_25
    0x34: iso_network_layer_34
    0x40: ibm_sna_40
    0x42: spanning_tree_bpdu
    0x4e: eia_rs_511_manufacturing_message_service
    0x54: iso_network_layer_oslan_2
    0x5e: isi_ip
    0x7e: iso_8208_x_25_over_802_2
    0x80: xns
    0x82: bacnet
    0x86: nestar
    0x8e: proway_iec955_active_station_list_maintenance
    0x98: arp
    0xaa: snapsub_network_access_protocol
    0xba: banyan_vines_ba
    0xbc: banyan_vines_bc
    0xd4: ibm_resource_management
    0xdc: ibm_dynamic_address_resolutionname_management
    0xe0: ipx_novell_netware
    0xf0: ibm_netbios
    0xf4: ibm_net_management
    0xf8: irplibm_remote_program_load_f8
    0xfa: ungermann_bass
    0xfc: irplibm_remote_program_load_fc
    0xfe: iso_network_layer_protocols
