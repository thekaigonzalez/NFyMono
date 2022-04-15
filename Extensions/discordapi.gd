static func change_activity(state, song, aes=false):
	var acti = Discord.Activity.new()
	acti.set_type(Discord.ActivityType.Listening)
	acti.set_state(state)
	acti.set_details(song)
	var assets = acti.get_assets()
	var activity = Discord.Activity.new()
	
	if aes:
		assets.set_small_image("win95")
		assets.set_small_text("Experiencing the aesthetic . . .")


	assets.set_large_image("nfymono")
	assets.set_large_text("Listening with NFy MONO")
	Discord.activity_manager.update_activity(acti)

static func parse_timer(time):
		var seconds = fmod(time,60)
		var minutes = fmod(time, 3600) / 60
		
		var secsx = ""
		if seconds < 10:
			secsx += "0"
		secsx += str(int(seconds))
		
		var t = ""
		
		t = str(int(minutes)) + ":" + secsx
		return t
