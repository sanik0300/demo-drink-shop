window.VerificationCodeForm = function VerificationCodeForm(props) 
{
    const [codeOk, setCodeOk] = React.useState(true);
    const [emailValid, setEmailValid] = React.useState(true);

    const [blockSecondsLeft, setBlockSecondsLeft] = React.useState(0);

    const [passStrength, setPassStrength] = React.useState(0);

    const emailInput = React.useRef(null);
    const resendTextP = React.useRef(null);

    const resendBlockIntervalRef = React.useRef(null);
    
    function onEmailInputChanged(e) 
    {
        var result = e.target.value.length > 0 && e.target.validity.valid;
        setEmailValid(result); 
    }

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

        var formData = new FormData();
        formData.append('emailTo', emailInput.current.value);

        await fetch("SendVerificationCode", {
            method: 'POST',
            body: formData
        })
        .then(async (response) => {
            if(response.ok) {
                startBlockingButton();
            }
        })
    }

    async function onCodeSubmit() {
        await fetch("VerifyPasswordChange", {
            method: 'PUT',
            body: new FormData(emailInput.current.parentElement)
        })
        .then(async (response) => {
            if(response.ok) {
                followRedirect(response.url)
            }
            else {
                var txt = await response.text();
                console.log(txt)
            }
        })        
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

                    <p style={(blockSecondsLeft > 0? undefined : {visibility: 'hidden'})} ref={resendTextP}>
                        Sending again possible in {blockSecondsLeft} seconds</p>
                </div>                       

                <h5>Enter the code from email</h5>

                <input name="code" type="number" placeholder="XXXX"  
                    min="1000" max="9999" 
                    style={codeOk? undefined : errorStyles}
                    onChange={(e) => {setCodeOk(e.target.validity.valid)}}/>
                
                <h5>new password</h5>
                <PasswordBox passwordStrengthCallback={(s)=>setPassStrength(s)} strengthCalculator={calculatePassStrength}/>
                <progress type="progress" min="0" max="4" value={Math.floor(passStrength).toString()}
                          style={(passStrength == 0? {visibility: 'hidden'} : undefined)}></progress>

                <button type="button" disabled={!codeOk || passStrength <= 1}
                        onClick={onCodeSubmit}>Submit</button>
            </form>
        </div>
    )
}