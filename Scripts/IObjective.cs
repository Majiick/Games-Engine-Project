using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObjective {
    bool IsTargeted();
    void SetTargeted(bool s);
}
