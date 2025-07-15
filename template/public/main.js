import WorkflowContainer from "./workflow.js"

export default {
    defaultTheme: 'light',
    iconLinks: [{
        icon: 'github',
        href: 'https://github.com/bonsai-rx/neuro',
        title: 'GitHub'
    }],
    start: () => {
        WorkflowContainer.init();
    }
}
