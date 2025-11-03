const domContainer = document.querySelector('#react-here');

const firstPurposeLogin = domContainer.dataset.purpose === 'login';
const WhatToRender = window.AccountFormWrapper;

ReactDOM.render(<WhatToRender loginMode={firstPurposeLogin}/>, domContainer);