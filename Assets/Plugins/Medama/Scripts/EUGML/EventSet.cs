using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Medama.EUGML {

    public class EventSet {
        public UnityAction<BaseEventData> action1 = null;
        public UnityAction action2 = null;
        public EventTriggerType triggerType = EventTriggerType.PointerClick;
        public EventSet(UnityAction<BaseEventData> action, EventTriggerType triggerType) {
            this.action1 = action;
            this.triggerType = triggerType;
        }
        public EventSet(UnityAction action, EventTriggerType triggerType) {
            this.action2 = action;
            this.triggerType = triggerType;
        }
    }

}
