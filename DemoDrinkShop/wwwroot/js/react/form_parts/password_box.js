window.PasswordBox = function PasswordBox(props) {

    const [visibility, setVisibility] = React.useState(false);
    const [empty, setEmpty] = React.useState(false);

    function onPasswordTextChanged(e) 
    {
        let pass = e.target.value;
        let updEmpty = pass.length == 0 || pass.indexOf(' ')>=0

        setEmpty(updEmpty);
        
        if(updEmpty) {
            props.passwordStrengthCallback(0);
            return;
        }
        if(pass.length<8) {
            props.passwordStrengthCallback(1);
            return;
        }

        if(props.strengthCalculator) {
            props.passwordStrengthCallback(props.strengthCalculator(pass));
            return
        }

        props.passwordStrengthCallback(2);
    }

    return(
        <div>
            <input id="pass" name="password"
                   type={(visibility? "text":"password")}
                   onChange={onPasswordTextChanged}
                   style={(empty? errorStyles : undefined)}/>
            <button onClick={(e) => {setVisibility(!visibility); e.preventDefault();}}>{(visibility? "hide" : "show")}</button>
        </div>
    )
}