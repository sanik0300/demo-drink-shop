window.VerificationCodeForm = function VerificationCodeForm(props) 
{
    const [codeOk, setCodeOk] = React.useState(true);
    const [emailValid, setEmailValid] = React.useState(true);

    const [blockSecondsLeft, setBlockSecondsLeft] = React.useState(0);

    const emailInput = React.useRef(null);
    const resendTextP = React.useRef(null);

    const resendBlockIntervalRef = React.useRef(null);

    const hiddenTextStyle = {visibility: 'hidden'}

    function startBlockingButton() 
    {
        setBlockSecondsLeft(30);

        resendBlockIntervalRef.current = setInterval(() => {
            
            setBlockSecondsLeft(prev => {
                const upd = prev - 1;
                if (upd <= 0) {
                    clearInterval(resendBlockIntervalRef.current);
                    return 0;
                }
                return upd;
            });

        }, 1000); //1000 ms update
    }

    const request2faCode = async () => {

        await fetch("SendVerificationCode", {
            method: 'POST',
            body: new FormData(emailInput.current.parentElement)
        })
        .then(async (response) => {
            if(response.ok) {
                startBlockingButton();
            }
        })
    }

    function onEmailInputChanged(e) 
    {
        var result = e.target.value.length > 0 && e.target.validity.valid;
        setEmailValid(result); 
    }

    React.useEffect(() => {
        async function prependEmail() {
            emailInput.current.value = await fetchTextData("GetMyEmail");
        }
        prependEmail();
    }, []);

    return(
        <div>
            <form>
                <h5>email where to send verification code</h5>
                <input type="email" name="emailTo" ref={emailInput}
                       style={emailValid? undefined : errorStyles}
                       onChange={onEmailInputChanged}/>
                <div>
                    <button type="button" onClick={request2faCode}
                            disabled={!emailValid || blockSecondsLeft > 0}>Send</button>

                    <p style={(blockSecondsLeft > 0? undefined : hiddenTextStyle)} ref={resendTextP}>
                        Sending again possible in {blockSecondsLeft} seconds</p>
                </div>                       
            </form>
            <form>
                <h5>Enter the code from email</h5>

                <input name="code2fa" type="number" placeholder="XXXX"  
                    min="1000" max="9999" 
                    style={codeOk? undefined : errorStyles}
                    onChange={(e) => {setCodeOk(e.target.validity.valid)}}/>
                 
                <input type="submit" disabled={!codeOk} value="Submit"></input>
            </form>
        </div>
    )
}