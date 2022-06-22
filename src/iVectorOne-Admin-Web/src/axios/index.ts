import axios from 'axios';

const ApiCall = axios.create({
  baseURL: 'https://api.ivectorone.com/admin',
});

export default ApiCall;
