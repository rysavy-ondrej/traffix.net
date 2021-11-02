meta:
  id: arp_packet
  title: Address Resolution Protocol
  endian: be
seq:
    - id: hardware_type
      type: u2
    - id: protocol_type
      type: u2  
    - id: hardware_addresslen 
      type: u1
    - id: protocol_addresslen
      type: u1
    - id: op_code
      type: u2
      enum: arp_op_code
    - id: sender_hardware_address
      size: hardware_addresslen
    - id: sender_protocol_address
      size: protocol_addresslen     
    - id: target_hardware_address
      size: hardware_addresslen
    - id: target_protocol_address
      size: protocol_addresslen 
enums:
    arp_op_code:
        1:  request
        2:  response
        3:  request_reverse
        4:  reply_reverse
        5:  drarp_request
        6:  drarp_reply
        7:  drarp_error
        8:  inarp_request
        9:  inarp_reply
        10:  arp_nak
        11:  mars_request
        12:  mars_multi
        13:  mars_mserv
        14:  mars_join
        15:  mars_leave
        16:  mars_nak
        17:  mars_unserv
        18:  mars_sjoin
        19:  mars_sleave
        20:  mars_grouplist_request
        21:  mars_grouplist_reply
        22:  mars_redirect_map
        23:  mapos_unarp

