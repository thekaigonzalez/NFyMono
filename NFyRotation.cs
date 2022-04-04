// NFY ROTATION
// multiple songs played in sequencial order
using System;
public class NFyRotation {  
    private string[] array_value = {};
    private int current_index = 0;

    public NFyRotation(string[] str) {
        array_value = str;
    } 

    public NFyRotation() {

    } 

    public void resetIndex() {
        current_index = 0;
    }

    public string getCurrentSong() {
        if (current_index < array_value.Length) {
            return array_value[current_index];
        } else {
            return "N";
        }
    }

    public string peekNext() {
        return array_value[current_index++];
    }

    public bool Dull() {
        return array_value.Length == 0;
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