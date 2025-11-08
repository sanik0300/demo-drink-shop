
const LimitedTextBox = window.LimitedTextBox;

window.PhoneInput = function PhoneInput(props) {

    let openedH = 100, foldedSize = 1, openedSize = 8, 
        fontSize = 18, 
        widthFromFont = (fontSize*5)+'pt';
    const foldedH = React.useRef(-1),
          yDiff = React.useRef(0);

    const [selectSize, setSelectSize] = React.useState(1);
    const [dropdownHeight, setdropdownHeight] = React.useState(foldedH.current);
    const [countriesList, setCountriesList] = React.useState([])

    const me = React.useRef(null),

          leftTextRef=React.useRef(null),
          rightTextRef=React.useRef(null),

          countryOk = React.useRef(false),
          restPhoneOk = React.useRef(false);
          
    const phoneCodeRef = React.useRef(undefined);

    function getInputsTop() {
        return Math.sign(dropdownHeight - foldedH.current)*yDiff.current
    }
    const inputStyles = {
        position: 'relative', top: getInputsTop(), display: 'inline-block'
    },
    selectStyles = {
        height: dropdownHeight+'px', 
        'padding': '2px', 
        'fontSize': fontSize+'pt',
        'maxWidth': widthFromFont
    }
    
    function validateOnlyDigits(text) {

        for (let i = 0; i < text.length; i++) 
        {
            if(!isDigit(text[i])) { return false }
        }
        return true
    }

    function toggleSizing(e, opened) 
    {   
        if(foldedH.current<=0) 
        {
            foldedH.current = e.nativeEvent.target.offsetHeight;
            yDiff.current = document.getElementsByName('myname')[0].offsetHeight - openedH - 3
        }
        setSelectSize(opened? openedSize : foldedSize);

        setdropdownHeight(opened? openedH : foldedH.current);
    }

    function onCountryCodeSelected(e) 
    {
        toggleSizing(e, false); 
        e.target.blur();
        //console.log(e.target.value);
        phoneCodeRef.current = e.target.value

        countryOk.current = true
        onOneBoxChanged();
    }

    function onOneBoxChanged() 
    {
        var result = countryOk.current && restPhoneOk.current;
        if(result == props.totalState.phoneOk) {return}
        
        props.totalState.phoneOk = result;
        props.totalStateSetter(props.totalState);
    }

    async function fetchCountries() {
        let jsontxt = await fetchTextData('/js/react/csvjson_countries.json')
        let jsonCountries = JSON.parse(jsontxt)
        jsonCountries.forEach((country) => country.emoji = unicodeToEmoji(country.unicode) );
        setCountriesList(jsonCountries)
    }
    
    React.useEffect(() => {

            leftTextRef.current = me.current.children[0];
            rightTextRef.current = me.current.children[1];
            fetchCountries()
        }, []
    )

    return(
        <div style={{'overflowY': 'visible', 'maxHeight': foldedH.current}}>
            <select id="countryselector" style={selectStyles} 
                    size={selectSize} className="emoji-part"
                    onFocus={(e) => toggleSizing(e, true)} 
                    onBlur={(e) => toggleSizing(e, false)}
                    onChange={onCountryCodeSelected}>

                {countriesList.map((country, index) => (
                    <option key={index} value={country.dialCode} className="emoji-part">{country.alpha3 + ' ' + country.emoji}</option>
                ))}
            </select>
            
            <div style={inputStyles} ref={me}>
                <LimitedTextBox id="countryInput" maxLength="6" undeletableStart="+"
                                propertyName="countryCode"
                                filterFunc={validateOnlyDigits}
                                rightFocusCallback={()=>{rightTextRef.current.focus();}}
                                style={{maxWidth: widthFromFont}}
                                textFromOutside={phoneCodeRef.current}
                                validityCallback={(b) => { countryOk.current = b; onOneBoxChanged(); }}/>

                <LimitedTextBox id="restOfPhoneInput" style={inputStyles} maxLength="12"
                                propertyName = "restOfPhone"
                                leftFocusCallback={()=>{leftTextRef.current.focus();}}
                                filterFunc={validateOnlyDigits}
                                validityCallback={(b) => { restPhoneOk.current = b; onOneBoxChanged(); }}/>
            </div>
        </div>
    )
}