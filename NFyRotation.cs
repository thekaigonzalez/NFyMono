// NFY ROTATION
// multiple songs played in sequencial order
using System;
using System.Linq;
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

    public void setIndex(int id) {
        current_index = id;
    }

    public string getCurrentSong() {
        if (array_value.ElementAtOrDefault(current_index) != null) {
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

    public string[] getArray() {
        return array_value;
    }
    public int getSize() {
        return array_value.Length;
    }


    public int currentIndex() {
        return current_index;
    }
    public bool nextExists() {
        if (array_value.ElementAtOrDefault(current_index+1) != null) {
            return true;
        }
        return false;
    }



}