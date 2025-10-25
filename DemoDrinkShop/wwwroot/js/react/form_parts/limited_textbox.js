window.LimitedTextBox = function LimitedTextBox(props) {
  
    const [canInput, setCanInput] = React.useState(true);
    const [text, setText] = React.useState("");
    const [maxW, setMaxW] = React.useState(undefined);

    const me = React.useRef(null);
    const lastConsumedText = React.useRef("")


    function getPrefixLength() 
    {
        if(!props.undeletableStart) {return 0}
        return props.undeletableStart.length;
    }

    function limitLength() 
    {
        if(!props.maxLength) {return}

        var lenToMultiply = Number(props.maxLength) + getPrefixLength();

        var computedStyleFS = window.getComputedStyle(me.current, null).getPropertyValue("font-size");
        var numberFS = Number(computedStyleFS.substring(0, computedStyleFS.length-2));
        setMaxW(numberFS/2*lenToMultiply + 'px');
    }
    

    function handleTextChange(e) 
    {
        var longerTextToCheck = props.undeletableStart?
                                        e.target.value.substring(props.undeletableStart.length)
                                        : e.target.value;
        var boolResult;
        if(e.nativeEvent.data != null) 
        {
            boolResult = canInput && props.filterFunc(e.nativeEvent.data)
        }
        else {
            boolResult = props.filterFunc(longerTextToCheck)
        }
        if(props.validityCallback) {
            props.validityCallback(boolResult && longerTextToCheck.length > 0)
        }
        
        setCanInput(boolResult)
        setText(longerTextToCheck)
    }

    function handleKeyDown(e) {

        if((e.keyCode == 37 || e.keyCode == 8) && e.target.selectionStart<=getPrefixLength()) 
        {   
            if(props.leftFocusCallback) {
                props.leftFocusCallback();
            }
            e.preventDefault();
            return
        }
        if(props.rightFocusCallback) 
        {
            var enterRightShift = e.keyCode == 13 && e.target.value.length > getPrefixLength();
            var rightArrowShift = e.keyCode == 39 && e.target.selectionStart >= e.target.value.length;

            if(enterRightShift || rightArrowShift)
            {
                props.rightFocusCallback()
                return
            }
        }
        var pref = getPrefixLength();
        if(e.target.selectionStart < pref) {
            e.target.selectionStart = pref;
        }
    }

    React.useEffect(limitLength, [0]);
    if(props.textFromOutside && props.textFromOutside != lastConsumedText.current) {
        setText(props.textFromOutside);
        setCanInput(true)
        lastConsumedText.current = props.textFromOutside
    }

    return (
        <input type={(props.inputtype? props.inputtype : "text")}
               name="myname" ref={me}
               value={(props.undeletableStart? props.undeletableStart + text : text)}
               maxLength={Number(props.maxLength)+getPrefixLength()}
               onInput={handleTextChange}
               onKeyDown={handleKeyDown}
               onChange={() => { }}
               style={{...(canInput? undefined : errorStyles), 'maxWidth': maxW}}/>
    );
}