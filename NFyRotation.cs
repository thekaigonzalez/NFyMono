// NFY ROTATION
// multiple songs played in sequencial order

public class NFyRotation {  
    private string[] array_value = {};
    private int current_index = 0;

    public NFyRotation(string[] str) {
        array_value = str;
    } 

    public string getCurrentSong() {
        return array_value[current_index];
    }

    public string peekNext() {
        return array_value[current_index++];
    }

    public void moveIndex() {
        current_index += 1;
    }

    public bool nextExists() {
        if (current_index < array_value.Length) {
            return true;
        }
        return false;
    }



}