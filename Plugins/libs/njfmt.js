// An implementation of NFy's format as seen in NJLog ({}, ["hello!"]) -> "hello!"

function Format(str, fmt) {
    let boo = false;
    let inddx = 0
    let final = "";
    for (let i = 0; i < str.length; ++i) {
        if (str[i] == "{" && !boo) boo = true;
        else if (str[i] == "}" && boo) {
            boo = false;
            final += fmt[inddx]

            inddx += 1
        } else {
            final += str[i]
        }
    }
    return final;
}