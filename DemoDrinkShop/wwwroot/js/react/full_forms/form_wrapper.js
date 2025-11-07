const LoginForm = window.LoginForm;
const RegistrationForm = window.RegistrationForm;

window.AccountFormWrapper = function AccountFormWrapper(props) {

    const [loginMode, setLoginMode] = React.useState(props.loginMode);

    const linkStyle = {'cursor': 'pointer'};

    return(
        <React.Fragment>
            <div className="myformheader"><h4>{(loginMode? "Log in" : "Register")}</h4></div> 
            {(loginMode? <LoginForm/> : <RegistrationForm/>)}
            
            <h5>
                <a href={(loginMode? "Register" : "Login")} style={linkStyle}>
                    {(loginMode? "Don't have an account yet - register" : "Already have an account - log in")}
                </a>
            </h5>
            {(loginMode? 
                <h5><a href="ChangePassword">Forgot password?</a></h5> : undefined
            )}
        </React.Fragment>
    ) 
}