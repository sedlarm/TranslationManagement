import React, { Component } from 'react';
import { TranslationJobs } from "./TranslationJobs";

export class Home extends Component {
    static displayName = Home.name;

    constructor(props) {
        super(props);
        this.state = { message: "", buttonEnabled: true };
    }

    render() {
        let contents = this.state.message;
        let buttonState = !this.state.buttonEnabled;
    return (
      <div>
            <h1>Welcome to Translation Management admin</h1>
            <p>Add new job for Translator #1</p>
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
                <button type="submit" disabled={buttonState}>Create</button>
            </form>
            <p>{contents}</p>
            <TranslationJobs unassigned="true" />
      </div>
    );
  }
    onCreateJob = (event) => {
        event.preventDefault();
        this.setState({ buttonEnabled: false, message: "executing operation..." })
        let jobInfo = {
            customerName: this.refs.CustomerName.value,
            originalContent: this.refs.ContentToTranslate.value
      };
      fetch("/api/jobs?translatorId=1", {
          method: 'POST',
          headers: { 'Content-type': 'application/json' },
          body: JSON.stringify(jobInfo)
      }).then(r => r.json()).then(res => {
          if (res) {
              this.setState({ message: "created job #" + res.id});
          }
      });
      this.setState({ buttonEnabled: true })
      event.target.reset();
  }

}
