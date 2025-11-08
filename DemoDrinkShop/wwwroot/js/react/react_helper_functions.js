const errorStyles = {
    outline: '3px solid red',
    outlineOffset: '0.5px'
};

function isDigit(c) {
    return c <= '9' && c >= '0';
}
function isLetter(c) {
    return c.toLowerCase() != c.toUpperCase();
}

async function fetchTextData(filePath) {

    const response = await fetch(filePath);
    if(response.ok) {
        var tx = await response.text()
        return tx;
    }
    else {
        console.log(response);
    }
}

function unicodeToEmoji(unicodeString) {
  return unicodeString
    .split(' ')
    .map(u => String.fromCodePoint(parseInt(u.replace('U+', ''), 16)))
    .join('');
}

function isSpecialCharacter(c) {
    return (c >= '!' && c <= '/') || (c >= ':' && c <= '@') || (c >= '[' && c <= '`') || (c >= '{' && c <= '~');
}

function calculatePassStrength(pwd) {

    var little=false, big=false, digits=false, special=false;
    for (let i = 0; i < pwd.length; i++) 
    {
        var c = pwd[i];
        if(isDigit(c)) {
            digits = true; 
        }
        else if (isSpecialCharacter(c)) {
            special = true;
        }
        else if(c == c.toUpperCase()) {
            big = true;
        }
        else {
            little = true;
        }

        if(little && big && digits && special) { break; }
    }
    var result = 0.5 + little + big + digits + special;
    return result
}
