tool
extends TileSet




func _is_tile_bound(drawn_id, neighbor_id):
	if drawn_id==find_tile_by_name("dirtEdge") and neighbor_id!=-1: return true
	else:
		return false
	
