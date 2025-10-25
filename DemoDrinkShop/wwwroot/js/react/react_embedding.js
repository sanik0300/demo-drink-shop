const domContainer = document.querySelector('#react-here');

const formPurpose = domContainer.dataset.purpose;
const WhatToRender = formPurpose === 'login' ? window.LoginForm : window.RegistrationForm;

ReactDOM.render(<WhatToRender/>, domContainer);