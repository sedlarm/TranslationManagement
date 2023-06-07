import React, { Component } from 'react';

export class Home extends Component {
    static displayName = Home.name;

    constructor(props) {
        super(props);
        this.state = { createdJob: null, message: "" };
    }

    render() {
        let contents = this.state.createdJob !== null ? "Job created, Id: " + this.state.createdJob : "";
    return (
      <div>
            <h1>Welcome to Translation Management admin</h1>
            <h2>Add new job</h2>
            <form onSubmit={this.onCreateJob}>
            <p>
                <label>Customer Name:
                    <input type="text" ref="CustomerName"></input>
                </label>
            </p>
            <p>
                <label>Content:
                    <input type="text" ref="ContentToTranslate"></input></label>
            </p>
            <button type="submit">Create</button>
            </form>
            <p>{contents}</p>
      </div>
    );
  }
    onCreateJob = (event) => {
        event.preventDefault();
        let jobInfo = {
            customerName: this.refs.CustomerName.value,
            originalContent: this.refs.ContentToTranslate.value
      };
      const request = new Request("http://localhost:5000/api/translators", {
          method: "get",
          mode: 'cors',
          headers: {
              Accept: "application/json"
          }
      });
      fetch('http://localhost:5000/api/jobs?translatorId=1', {
          method: 'POST',
          headers: { 'Content-type': 'application/json' },
          body: JSON.stringify(jobInfo)
      }).then(r => r.json()).then(res => {
          if (res) {
              this.setState({ createdJob: res.id });
          }
      });
      event.target.reset();
  }

}
