const LoginForm = window.LoginForm;
const RegistrationForm = window.RegistrationForm;

window.AccountFormWrapper = function AccountFormWrapper(props) {

    const [loginMode, setLoginMode] = React.useState(props.loginMode);

    return(
        <React.Fragment>
            <div className="myformheader"><h4>{(loginMode? "Log in" : "Register")}</h4></div> 
            {(loginMode? <LoginForm/> : <RegistrationForm/>)}

            <a href={(loginMode? "Register" : "Login")} style={{'cursor': 'pointer'}}>
                {(loginMode? "Don't have an account yet - register" : "Already have an account - log in")}
            </a>
        </React.Fragment>
    ) 
}