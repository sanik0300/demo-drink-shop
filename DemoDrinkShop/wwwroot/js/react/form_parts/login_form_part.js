const PhoneInput = window.PhoneInput;
const PasswordBox = window.PasswordBox;

window.LoginFormPart = function LoginFormPart(props) {
        
    const [emailValid, setEmailValid] = React.useState(true);
    const [passStrength, setPassStrength] = React.useState(0);
    
    const passProgressStyles = {
        visibility: (passStrength == 0? 'hidden' : undefined)
    }

    function onEmailInputChanged(e) 
    {
        var result = e.target.value.length > 0 && e.target.validity.valid;
        setEmailValid(result);        
        
        props.totalState.emailOk = result;
        props.totalStateSetter(props.totalState)
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
        var result = 0 + little + big + digits + special;
        return result
    }

    function onPassStrengthChanged(strength) 
    {
        setPassStrength(strength);
        props.totalState.passwordOk = strength > 1;
        props.totalStateSetter(props.totalState);
    }

    return(
        <div>      
            <div>
                <h5>phone</h5> 
                <PhoneInput totalState={props.totalState} totalStateSetter={props.totalStateSetter}/>               
            </div>
            <div>
                <h5>email</h5>
                <input type="email" name="email" 
                       style={emailValid? undefined : errorStyles}
                       onChange={onEmailInputChanged}/>
            </div>
            <div>
                <h5>password</h5> 
                <PasswordBox passwordStrengthCallback={onPassStrengthChanged}
                             strengthCalculator={(props.showPassStrength? calculatePassStrength : undefined)}/>
                {(props.showPassStrength?
                    <progress type="progress" min="0" max="4" value={passStrength.toString()}
                            style={passProgressStyles}></progress>
                    :
                    undefined
                )}
            </div>
        </div>
    )
}