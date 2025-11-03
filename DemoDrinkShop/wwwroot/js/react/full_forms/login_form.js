const LoginFormPart = window.LoginFormPart;

window.LoginForm = function LoginForm() {

    const prevLoginState = React.useRef({emailOk: false, phoneOk: false, passwordOk: false})
    const [loginState, setLoginState] = React.useState({emailOk: false, phoneOk: false, passwordOk: false});

    function loginStateSetter(st) 
    {
        if(prevLoginState.current.emailOk == st.emailOk && prevLoginState.current.phoneOk == st.phoneOk 
            && prevLoginState.current.passwordOk == st.passwordOk) { return }
        
        prevLoginState.current = this.totalState

        setLoginState({ emailOk: st.emailOk, phoneOk: st.phoneOk, passwordOk: st.passwordOk }); 
    }

    return(
        <form method="post" action="Login">
            <LoginFormPart showPassStrength={false}
                           totalState={loginState} totalStateSetter={loginStateSetter}/>

            <input type="submit" value="login" 
                   disabled={(!loginState.emailOk && !loginState.phoneOk) || !loginState.passwordOk}/>
        </form>
    )
}

//export default window.RegistrationForm;