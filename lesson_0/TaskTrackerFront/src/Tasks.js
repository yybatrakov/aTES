import React from 'react';
import { useState } from 'react';

class Tasks extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            tasks: null,
            login_input: null,
            user: null
        };
    }

    componentDidMount() {
        const headers = { 'Content-Type': 'application/json' }

        fetch(`http://localhost:32002/Tasks`, {
            headers
        })

        .then(response => response.json())
        .then(data => 
            this.setState({ tasks: data })
        );
    }

    auth(login)
    {
        const headers = { 'Content-Type': 'application/json' }

        fetch(`http://localhost:32000/Auth/Login?login=${login}`, {
            headers: headers         
        })

        .then(response => response.json())
        .then(data => 
            this.setState({ user: data })
        );

    }
    
    

    render() {
        const loginClick = () => {
            this.auth(this.state.login_input)
          }

        const { tasks, user } = this.state;
        return (
            <div>
                <lable>
                    <h5>Auth</h5>
                    <input onChange={e => this.setState({ login_input: e.target.value }) } />
                    <button onClick={loginClick}>Login</button>
                </lable>
                
                <div>
                    {user == null || 
                        <label>login: {user.login} role: {user.role}</label>
                    }
             
                </div>
                
                {user == null ||
                    <div>
                        <h5>Tasks</h5>
                        <table>
                            <tr key={"header"}>
                                { tasks == null || Object.keys(tasks[0]).map((key) => (
                                    <th>{key}</th>
                                    ))}
                                </tr>
                            { tasks == null || tasks.map((item) => (
                                <tr key={item.id}>
                                {Object.values(item).map((val) => (
                                    <td>{String(val)}</td>
                                ))}
                                </tr>
                            ))}
                        </table>
                    </div>
                }
            </div>
        );
    }
}

export default Tasks