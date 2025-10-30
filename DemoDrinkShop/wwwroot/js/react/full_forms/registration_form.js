const LimitedTextBox = window.LimitedTextBox;
const LoginFormPart = window.LoginFormPart;

window.RegistrationForm = function RegistrationForm() {

    const namePunctuation = "-`'",
          addressPunctuation = "-`'#( ),-./№";

    const prevLoginState = React.useRef({emailOk: false, phoneOk: false, passwordOk: false})
    const [loginState, setLoginState] = React.useState({emailOk: false, phoneOk: false, passwordOk: false});
    
    const emailCheck = React.useRef(null),
          phoneCheck = React.useRef(null);
    const [lastCheckedBox, setLastCheckedBox] = React.useState(-1);

    function loginStateSetter(st) 
    {
        if(prevLoginState.current.emailOk == st.emailOk && prevLoginState.current.phoneOk == st.phoneOk 
            && prevLoginState.current.passwordOk == st.passwordOk) { return }
        
        prevLoginState.current = this.totalState

        if(st.phoneOk != st.emailOk) {
            setLastCheckedBox(Number(st.phoneOk))
        }
        setLoginState({ emailOk: st.emailOk, phoneOk: st.phoneOk, passwordOk: st.passwordOk }); 
    }

    function validateCharForName(text) {
        for (let i = 0; i < text.length; i++) 
        {
            var c = text[i];
            if(isDigit(c)) { return false }

            if(c != ' ' && !isLetter(c) && namePunctuation.indexOf(c) == -1) {
                return false
            }
        }
        return true
    }
    function validateExceptSpecialCharacters(text) {
        for (let i = 0; i < text.length; i++) 
        {
            var c = text[i];
            if(!isLetter(c) && !isDigit(c) && addressPunctuation.indexOf(c)==-1) {
                return false
            }
        }
        return true
    }
    function toggleCheckboxes(target, other, val) 
    {
        other.checked = !target.checked
        setLastCheckedBox(val)
    }

    return(
        <form method="post" action="Register">
            <LoginFormPart showPassStrength={true} 
                           totalState={loginState} totalStateSetter={loginStateSetter}/>
            <div id="verify-options">
                <h5>Verify account by:</h5>

                <input type="checkbox" id="vm" ref={emailCheck}
                       disabled={!loginState.emailOk}
                       checked={loginState.emailOk && lastCheckedBox==0}
                       onClick={(e)=>{toggleCheckboxes(e.target, phoneCheck.current, 0)}}/>
                <label htmlFor="vm">by email</label>
            
                <br/>
                <input type="checkbox" id="vs" ref={phoneCheck}
                       disabled={!loginState.phoneOk}
                       checked={loginState.phoneOk && lastCheckedBox==1}
                       onClick={(e)=>{toggleCheckboxes(e.target, emailCheck.current, 1)}}/>
                <label htmlFor="vs">by sms (phone)</label>
            </div>
            <div>
                <h5>name</h5> 
                <LimitedTextBox filterFunc={validateCharForName} propertyName="name"/>               
            </div>
            <div>
                <h5>address</h5>
                <LimitedTextBox filterFunc={validateExceptSpecialCharacters} propertyName="address"/>
            </div>

            <input type="submit" value="register" 
                   disabled={(!loginState.emailOk && !loginState.phoneOk) || !loginState.passwordOk}/>
        </form>
    )
}