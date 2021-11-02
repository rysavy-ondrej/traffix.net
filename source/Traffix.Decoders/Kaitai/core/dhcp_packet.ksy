meta:
  id: dhcp_packet
  title: Dynamic Host Configuration Protocol (for IPv4)
  endian: be
seq:
    - id: op_code
      type: u1
      enum: dhcp_opcode 
    - id: hardware_type 
      type: u1
      enum: dhcp_hardware_type
    - id: hardware_address_length
      type: u1
    - id: hop_count
      type: u1
    - id: traqnsaction_id
      type: u4
    - id: seconds
      type: u2
    - id: flags      
      type: dhcp_flags
    - id: client_ip
      size: 4
    - id: your_ip
      size: 4
    - id: server_ip
      size: 4
    - id: relay_agent_ip
      size: 4
    - id: client_hardware_address
      size: 16
    - id: server_host_name
      type: strz
      encoding: ASCII
    - id: boot_file_name
      type: strz
      encoding: ASCII
    - id: magic_cookie
      size: 4    
    - id: options
      type: dhcp_option
      repeat: eos
types:         
    dhcp_flags:
        seq:
            - id: broadcast
              type: b1
            - id: reserved
              type: b15
    dhcp_option:
        seq:
            - id: code
              type: u1
              enum: dhcp_option_code 
            - id: len
              type: u1
            - id: body
              size: len
enums:
    dhcp_opcode:    
        1: request
        2: reply
    dhcp_hardware_type:
        1: ethernet 
        2: experimental_ethernet 
        3: amateur_radio
        4: proteon_pronet_token_ring 
        5: chaos 
        6: ieee_802 
        7: arcnet 
        8: hyperchannel 
        9: lanstar 
        10:  autonet_short_address 
        11:  localtalk 
        12:  localnet 
        13:  ultra_link 
        14:  smds 
        15:  frame_relay 
        16:  atm_16 
        17:  hdlc 
        18:  fibre_channel 
        19:  atm_19 
        20:  serial_line 
        21:  atm_21 
        22:  mil_std_188_220 
        23:  metricom 
        24:  ieee_1394p1995 
        25:  mapos 
        26:  twinaxial 
        27:  eui_64 
        28:  hiparp 
        29:  ip_and_arp_over_iso
        30:  arpsec 
        31:  ipsec_tunnel 
        32:  infiniband 
        33:  cai 
    # for option 53
    dhcp_message_type:
        1: discover
        2: offer
        3: request
        4: decline
        5: ack
        6: nak
        7: release
        8: inform
    dhcp_option_code:
        0: pad
        1: subnet_mask
        2: time_offset
        3: router
        4: time_server
        5: name_server
        6: domain_name_server
        7: log_server
        8: cookie_server
        9: lpr_server
        10: impress_server
        11: resource_location_server
        12: host_name
        13: boot_file_size
        14: merit_dump_file
        15: domain_name
        16: swap_server
        17: root_path
        18: extensions_path
        19: ip_forwarding_enable_disable
        20: non_local_source_routing_enable_disable
        21: policy_filter
        22: maximum_datagram_reassembly_size
        23: default_ip_time_to_live
        24: path_mtu_aging_timeout
        25: path_mtu_plateau_table
        26: interface_mtu
        27: all_subnets_are_local
        28: broadcast_address
        29: perform_mask_discovery
        30: mask_supplier
        31: perform_router_discovery
        32: router_solicitation_address
        33: static_route
        34: trailer_encapsulation
        35: arp_cache_timeout
        36: ethernet_encapsulation
        37: tcp_default_ttl
        38: tcp_keepalive_interval
        39: tcp_keepalive_garbage
        40: network_information_service_domain_40
        41: network_information_servers
        42: network_time_protocol_servers
        43: vendor_specific_information
        44: netbios_over_tcp_ip_name_server
        45: netbios_over_tcp_ip_datagram_distribution_server
        46: netbios_over_tcp_ip_node_type
        47: netbios_over_tcp_ip_scope
        48: x_window_system_font_server
        49: x_window_system_display_manager
        50: requested_ip_address
        51: ip_address_lease_time
        52: option_overload
        53: dhcp_message_type
        54: server_identifier
        55: parameter_request_list
        56: message
        57: maximum_dhcp_message_size
        58: renewal_t1_time_value
        59: rebinding_t2_time_value
        60: class_identifier
        61: client_identifier
        62: netware_ip_domain_name
        63: netware_ip_information
        64: network_information_service_domain_64
        65: network_information_service_servers
        66: tftp_server_name
        67: bootfile_name
        68: mobile_ip_home_agent
        69: simple_mail_transport_protocol_server
        70: post_office_protocol_server
        71: network_news_transport_protocol_server
        72: default_world_wide_web_server
        73: default_finger_server
        74: default_internet_relay_chat_server
        75: streettalk_server
        76: streettalk_directory_assistance_server
        77: user_class_information
        78: slp_directory_agent
        79: slp_service_scope
        81: fully_qualified_domain_name
        82: agent_circuit_id
        85: nds_servers
        86: nds_tree_name
        87: nds_context
        88: bcmcs_domain_name
        89: bcmcs_server_address
        90: authentication
        93: client_system
        94: client_network_device_interface
        95: lightweight_directory_access_protocol
        97: uuid_guid_based_client_identifier
        98: open_groups_user_authentication
        99: overall_format
        109: autonomous_system_number
        112: netinfo_parent_server_address
        113: netinfo_parent_server_tag
        114: url
        116: auto_configure
        117: name_service_search
        118: subnet_selection
        119: dns_domain_search_list
        120: sip_servers_dhcp_option
        121: classless_static_route_option
        122: cablelabs_client_configuration
        123: geoconf
        124: vendor_identifying_vendor_class
        125: vendor_identifying_vendor_specific
        126: extension_126
        127: extension_127
        249: classless_static_route
        250: continuation_option
        252: web_proxy_auto_detection_wpad
        255: end_of_options

