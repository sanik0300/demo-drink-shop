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

async function fetchJsonText(filePath) {

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
