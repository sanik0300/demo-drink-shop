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
        <form onSubmit={(e) => { e.preventDefault();
                                 onAjaxSubmit("Login", 'POST', new FormData(e.target))}}>
            <LoginFormPart showPassStrength={false}
                           totalState={loginState} totalStateSetter={loginStateSetter}/>

            <button type="submit"
                    disabled={(!loginState.emailOk && !loginState.phoneOk) || !loginState.passwordOk}
                    >Login</button>
        </form>
    )
}

//export default window.RegistrationForm;