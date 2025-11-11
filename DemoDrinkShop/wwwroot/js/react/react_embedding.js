const domContainer = document.querySelector('#react-here');

var p = domContainer.dataset.purpose;
switch(p) {
    case 'code': {
        const WhatToRender = window.VerificationCodeForm;
        ReactDOM.render(<WhatToRender/>, domContainer);
    }
        break;
    default: {
        const firstPurposeLogin = p === 'login';
        const WhatToRender = window.AccountFormWrapper;
        ReactDOM.render(<WhatToRender loginMode={firstPurposeLogin}/>, domContainer);
    }
        break;
}
