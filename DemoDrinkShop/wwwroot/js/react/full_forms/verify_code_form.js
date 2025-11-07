window.VerificationCodeForm = function VerificationCodeForm(props) 
{
    const [codeOk, setCodeOk] = React.useState(true);
    const [blockSecondsLeft, setBlockSecondsLeft] = React.useState(0);

    const resendBlockIntervalRef = React.useRef(null);

    const hiddenTextStyle = {visibility: 'hidden'}

    const request2faCode = () => {
        //fetch("Send2faCode");

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

    return(
        <form>
            <h5>Enter the code from email</h5>

            <input name="code2fa" type="number" placeholder="XXXX"  
                   min="1000" max="9999" 
                   style={codeOk? undefined : errorStyles}
                   onChange={(e) => {setCodeOk(e.target.validity.valid)}}/>
            
            <div>
                <button type="button" onClick={request2faCode}
                        disabled={blockSecondsLeft > 0}>Send</button>

                <p style={(blockSecondsLeft > 0? undefined : hiddenTextStyle)}>
                    Sending again possible in {blockSecondsLeft} seconds</p>
            </div>
            
            <input type="submit" disabled={!codeOk} value="Submit"></input>
        </form>
    )
}